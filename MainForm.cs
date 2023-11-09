using System.Diagnostics;
using PacketDotNet;
using SharpPcap;

namespace nps
{
    public partial class MainForm : Form
    {
        private CaptureDeviceList devices;
        private ICaptureDevice? selectedDevice = null;
        private List<RawCapture> capturedPackets = new();

        private System.Threading.Thread backgroundThread;
        private bool backgroundThreadStop;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            devices = CaptureDeviceList.Instance;
            foreach (var dev in devices)
            {
                selectDevices.Items.Add(dev.Description);
            }
            selectDevices.SelectedIndex = 0;

            listPacket.View = View.Details;
            listPacket.Columns.Add("No", 50, HorizontalAlignment.Left);
            listPacket.Columns.Add("Time", 150, HorizontalAlignment.Left);
            listPacket.Columns.Add("Source", 150, HorizontalAlignment.Left);
            listPacket.Columns.Add("Destination", 150, HorizontalAlignment.Left);
            listPacket.Columns.Add("Length", 80, HorizontalAlignment.Left);
            listPacket.Columns.Add("网络层协议", 100, HorizontalAlignment.Left);
            listPacket.Columns.Add("传输层协议", 100, HorizontalAlignment.Left);
            listPacket.Columns.Add("应用层协议", 100, HorizontalAlignment.Left);

            listFilter.View = View.Details;
            listFilter.Columns.Add("No", 50, HorizontalAlignment.Left);
            listFilter.Columns.Add("Time", 150, HorizontalAlignment.Left);
            listFilter.Columns.Add("Source", 150, HorizontalAlignment.Left);
            listFilter.Columns.Add("Destination", 150, HorizontalAlignment.Left);
            listFilter.Columns.Add("Length", 80, HorizontalAlignment.Left);
            listFilter.Columns.Add("网络层协议", 100, HorizontalAlignment.Left);
            listFilter.Columns.Add("传输层协议", 100, HorizontalAlignment.Left);
            listFilter.Columns.Add("应用层协议", 100, HorizontalAlignment.Left);

            comboBox1.Items.Add("所有");
            comboBox1.Items.Add("IPv4");
            comboBox1.Items.Add("IPv6");
            comboBox2.Items.Add("所有");
            comboBox2.Items.Add("Tcp");
            comboBox2.Items.Add("Udp");
            comboBox3.Items.Add("所有");

            btnStop.Enabled = false;
        }

        private void btnDeviceDetails_Click(object sender, EventArgs e)
        {
            ICaptureDevice dev = devices[selectDevices.SelectedIndex];
            MessageBox.Show(dev.ToString(), "info");
        }

        private PacketArrivalEventHandler arrivalEventHandler;
        private void btnStart_Click(object sender, EventArgs e)
        {
            //开始捕获,创建线程backgroundThread
            capturedPackets = new List<RawCapture>();
            backgroundThread = new Thread(BackgroundThread);
            backgroundThreadStop = false;
            backgroundThread.Start();
            

            selectedDevice = devices[selectDevices.SelectedIndex];
            arrivalEventHandler = new PacketArrivalEventHandler(Packet_ArrivalEventHandler);
            selectedDevice.OnPacketArrival += arrivalEventHandler;
            selectedDevice.Open();
            selectedDevice.StartCapture();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private readonly object QueueLock = new();
        void Packet_ArrivalEventHandler(object sender, PacketCapture e)
        {
            lock (QueueLock)
            {
                capturedPackets.Add(e.GetPacket());
            }

        }
        private List<string> protocols = new();

        private void BackgroundThread()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            listPacket.Items.Clear();
            var count = 0;
            while (true)
            {
                bool shouldSleep = true;
                lock (QueueLock)
                {
                    //新的数据包可用
                    if (count < capturedPackets.Count)
                        shouldSleep = false;
                }
                if (shouldSleep)
                {
                    if (backgroundThreadStop == true)
                        break;
                    else
                        Thread.Sleep(50);
                }
                else
                {
                    var rawPacket = capturedPackets[count];
                    Packet packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

                    ListViewItem item = new();
                    //NO
                    item.SubItems[0].Text = count.ToString();
                    //Time
                    item.SubItems.Add(rawPacket.Timeval.Date.ToLocalTime().TimeOfDay.ToString());
                    var eth = (EthernetPacket)packet;
                    //Source Destination
                    if (eth.HasPayloadPacket)
                    {
                        //以太网帧中包含 IP 数据包
                        if (eth.PayloadPacket is IPPacket)
                        {
                            IPPacket ippacket = (IPPacket)eth.PayloadPacket;
                            item.SubItems.Add(ippacket.SourceAddress.ToString());
                            item.SubItems.Add(ippacket.DestinationAddress.ToString());
                        }
                        //以太网帧中包含 ARP 数据包
                        else if (eth.PayloadPacket is ArpPacket)
                        {
                            ArpPacket arppacket = (ArpPacket)eth.PayloadPacket;
                            item.SubItems.Add(arppacket.SenderProtocolAddress.ToString());
                            item.SubItems.Add(arppacket.TargetProtocolAddress.ToString());
                        }
                    }
                    else //太网帧中没有有效的负载数据包
                    {
                        item.SubItems.Add(eth.SourceHardwareAddress.ToString());
                        item.SubItems.Add(eth.DestinationHardwareAddress.ToString());

                    }

                    //Length
                    item.SubItems.Add(rawPacket.Data.Length.ToString());
                    //数据包类型
                    item.SubItems.Add(eth.Type.ToString());
                    if (eth.PayloadPacket is IPPacket)
                    {
                        IPPacket ippacket = (IPPacket)eth.PayloadPacket;
                        //传输层协议
                        item.SubItems.Add(ippacket.Protocol.ToString());
                    }


                    item.SubItems.Add(JudgeProtocol(packet)); //应用层协议
                    listPacket.Items.Add(item);
                    count++;

                    string protocol = JudgeProtocol(packet);
                    if (protocol != null && protocol != "")
                    {
                        if (!protocols.Contains(protocol))
                        {
                            protocols.Add(protocol);
                            comboBox3.Items.Add(protocol);
                        }
                    }
                }
            }
        }

        private static string JudgeProtocol(Packet packet)
        {
            String protocol = "";
            TcpPacket tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
            if (tcpPacket != null)
            {
                if (tcpPacket.DestinationPort == 80 || tcpPacket.SourcePort == 80)
                    protocol = "HTTP";
                else if (tcpPacket.DestinationPort == 443)
                    protocol = "HTTPS";
                else if (tcpPacket.DestinationPort == 25)
                    protocol = "SMTP";
            }

            UdpPacket udpPacket = packet.Extract<PacketDotNet.UdpPacket>();
            if (udpPacket != null)
            {
                if (udpPacket.DestinationPort == 53)
                    protocol = "DNS";
            }
            return protocol;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            selectedDevice.StopCapture();
            selectedDevice.Close();
            selectedDevice.OnPacketArrival -= arrivalEventHandler;
            selectedDevice = null;
            btnStart.Enabled = true;
            btnStop.Enabled = false;

            backgroundThreadStop = true;
            //等待线程完成执行
            backgroundThread.Join();
        }

        private void btnFilterStart_Click(object sender, EventArgs e)
        {
            //确定筛选
            listFilter.Items.Clear();
            Update_listFilter();
            listPacket.Visible = false;
            listFilter.Visible = true;
        }

        private void btnFilterStop_Click(object sender, EventArgs e)
        {
            //取消筛选
            listPacket.Visible = true;
            listFilter.Visible = false;
        }

        private List<RawCapture> filterRawPacket = new();
        private void Update_listFilter()
        {
            string selected1 = "";
            string selected2 = "";
            string selected3 = "";
            if (comboBox1.SelectedIndex > 0)
            {
                selected1 = comboBox1.SelectedItem.ToString();
            }
            if (comboBox2.SelectedIndex > 0)
            {
                selected2 = comboBox2.SelectedItem.ToString();
            }
            if (comboBox3.SelectedIndex > 0)
            {
                selected3 = protocols[comboBox3.SelectedIndex - 1];
            }

            filterRawPacket = new List<RawCapture>();
            int count;
            lock (QueueLock)
            {
                count = capturedPackets.Count;
            }
            for (int i = 0; i < count; i++)
            {
                RawCapture rawPacket = capturedPackets[i];
                Packet packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                EthernetPacket eth = (EthernetPacket)packet;

                if (selected1 != "")
                {
                    if (selected1 != eth.Type.ToString())
                        continue;
                }
                if (selected2 != "")
                {
                    if (eth.PayloadPacket is IPPacket)
                    {
                        IPPacket ippacket = (IPPacket)eth.PayloadPacket;
                        if (selected2 != ippacket.Protocol.ToString())
                            continue;
                    }
                }
                if (selected3 != "")
                {
                    if (selected3 != JudgeProtocol(packet))
                        continue;
                }

                filterRawPacket.Add(rawPacket);

                ListViewItem sourceItem = listPacket.Items[i];
                ListViewItem newItem = (ListViewItem)sourceItem.Clone();

                // 将新的 ListViewItem 添加到目标 ListView 控件
                listFilter.Items.Add(newItem);
            }
        }

        private int select_index = -1;

        private void listPacket_SelectedIndexChanged(object sender, EventArgs e)
        {
            int count = listPacket.SelectedItems.Count;
            if (count > 0)
            {
                select_index = int.Parse(listPacket.SelectedItems[0].SubItems[0].Text);
                show_detail();
            }
        }

        private void listFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            int count = listFilter.SelectedItems.Count;
            if (count > 0)
            {
                select_index = int.Parse(listFilter.SelectedItems[0].SubItems[0].Text);
                show_detail();
            }
        }

        //调用一些库函数 串联起来
        private void show_detail()
        {
            RawCapture rawPacket = capturedPackets[select_index];
            Packet packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            EthernetPacket eth = (EthernetPacket)packet;
            var tcpPacket = packet.Extract<TcpPacket>();
            var udpPacket = packet.Extract<UdpPacket>();

            int count = 0;
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("原始数据包:");
            treeView1.Nodes[count].Nodes.Add("链路层类型：" + rawPacket.LinkLayerType.ToString());
            treeView1.Nodes[count].Nodes.Add("捕获时间：" + rawPacket.Timeval.Date.ToLocalTime().TimeOfDay);
            treeView1.Nodes[count].Nodes.Add("长度：" + rawPacket.Data.Length.ToString());
            count++;

            treeView1.Nodes.Add("链路层");
            treeView1.Nodes[count].Nodes.Add("源MAC地址：" + eth.SourceHardwareAddress.ToString());
            treeView1.Nodes[count].Nodes.Add("目标MAC地址：" + eth.DestinationHardwareAddress.ToString());
            treeView1.Nodes[count].Nodes.Add("上层协议：" + eth.Type.ToString());
            count++;

            treeView1.Nodes.Add("网络层");
            if (eth.PayloadPacket is ArpPacket)
            {
                ArpPacket arppacket = (ArpPacket)eth.PayloadPacket;
                treeView1.Nodes[count].Nodes.Add("协议类型：ARP");
                treeView1.Nodes[count].Nodes.Add("操作：" + arppacket.Operation);
                treeView1.Nodes[count].Nodes.Add("协议地址类型：" + arppacket.ProtocolAddressType);
                treeView1.Nodes[count].Nodes.Add("协议地址长度：" + arppacket.ProtocolAddressLength);
                treeView1.Nodes[count].Nodes.Add("发送者协议地址：" + arppacket.SenderProtocolAddress.ToString());
                treeView1.Nodes[count].Nodes.Add("目标协议地址：" + arppacket.TargetProtocolAddress.ToString());
                count++;
            }

            if (eth.PayloadPacket is IPPacket)
            {
                IPPacket ippacket = (IPPacket)eth.PayloadPacket;
                treeView1.Nodes[count].Nodes.Add("协议类型：" + eth.Type.ToString());
                treeView1.Nodes[count].Nodes.Add("数据包长度：" + ippacket.TotalLength);
                treeView1.Nodes[count].Nodes.Add("版本：" + ippacket.Version);
                treeView1.Nodes[count].Nodes.Add("首部长度：" + ippacket.HeaderLength);
                treeView1.Nodes[count].Nodes.Add("源ip地址：" + ippacket.SourceAddress.ToString());
                treeView1.Nodes[count].Nodes.Add("目的ip地址：" + ippacket.DestinationAddress.ToString());
                treeView1.Nodes[count].Nodes.Add("生存时间：" + ippacket.TimeToLive.ToString());
                treeView1.Nodes[count].Nodes.Add("上层协议：" + ippacket.Protocol.ToString());

                if (ippacket is IPv4Packet)
                {
                    IPv4Packet ipv4 = (IPv4Packet)ippacket;
                    treeView1.Nodes[count].Nodes.Add("IPv4数据包");
                    treeView1.Nodes[count].Nodes[8].Nodes.Add("校验码：" + ipv4.Checksum);
                    treeView1.Nodes[count].Nodes[8].Nodes.Add("校验码状态：" + ipv4.ValidChecksum.ToString());
                    treeView1.Nodes[count].Nodes[8].Nodes.Add("分片标志：" + ipv4.FragmentFlags);
                    treeView1.Nodes[count].Nodes[8].Nodes.Add("片偏移：" + ipv4.FragmentOffset);
                    treeView1.Nodes[count].Nodes[8].Nodes.Add("标识：" + ipv4.Id);
                    treeView1.Nodes[count].Nodes[8].Nodes.Add("TOS服务类型：" + ipv4.TypeOfService);
                }
                if (ippacket is IPv6Packet)
                {
                    IPv6Packet ipv6 = (IPv6Packet)ippacket;
                    treeView1.Nodes[count].Nodes.Add("IPv6数据包：");
                    treeView1.Nodes[count].Nodes[8].Nodes.Add("流量类型：" + ipv6.TrafficClass);
                    treeView1.Nodes[count].Nodes[8].Nodes.Add("流标签：" + ipv6.FlowLabel);
                }
                count++;
            }

            treeView1.Nodes.Add("传输层");
            if (tcpPacket != null)
            {
                treeView1.Nodes[count].Nodes.Add("协议类型：Tcp");
                treeView1.Nodes[count].Nodes.Add("源端口：" + tcpPacket.SourcePort);
                treeView1.Nodes[count].Nodes.Add("目的端口：" + tcpPacket.DestinationPort);
                treeView1.Nodes[count].Nodes.Add("长度：" + tcpPacket.TotalPacketLength);
                treeView1.Nodes[count].Nodes.Add("序列号：" + tcpPacket.SequenceNumber);
                treeView1.Nodes[count].Nodes.Add("ACK序列号：" + tcpPacket.AcknowledgmentNumber);
                treeView1.Nodes[count].Nodes.Add("偏移量：" + tcpPacket.DataOffset);

            }

            if (udpPacket != null)
            {
                treeView1.Nodes[count].Nodes.Add("协议类型：Udp");
                treeView1.Nodes[count].Nodes.Add("源端口：" + udpPacket.SourcePort);
                treeView1.Nodes[count].Nodes.Add("目的端口：" + udpPacket.DestinationPort);
                treeView1.Nodes[count].Nodes.Add("长度：" + udpPacket.Length);
            }
            treeView1.EndUpdate();
        }
    }
}
