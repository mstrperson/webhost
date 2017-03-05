using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;
using System.Collections;

namespace WebhostMySQLConnection.AccountManagement
{
    public class DeviceRegistration
    {
        protected static String AcademicDHCPServer = "192.168.98.4";

        public static String StudentSubnet = "192.168.20.0";
        public static String ProctorSubnet = "192.168.16.0";
        public static String FacultySubnet = "192.168.64.0";
        public static String LibrarySubnet = "192.168.98.0";
        public static String GuestSubnet = "192.168.74.0";

        public class ipaddr : IComparable
        {
            public string ip { get; protected set; }
            protected int[] digits;

            public ipaddr(String ip)
            {
                this.ip = ip;
                this.digits = IPStringToDigits(ip);
            }

            public override String ToString() { return ip; }

            public ipaddr next()
            {
                return new ipaddr(IncrementIP(this.ip));
            }

            public int CompareTo(object other)
            {
                if(other is ipaddr)
                {
                    return (IPCompare(this.digits, ((ipaddr)other).digits));
                }
                else
                {
                    return 0;
                }
            }

            public override bool Equals(object other)
            {
                if (other is ipaddr)
                {
                    return this.ip.Equals(((ipaddr)other).ip);
                }
                else return false;
            }

            public static int IPCompare(int[] left, int[] right)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (left[i] != right[i])
                    {
                        int diff = (left[i] - right[i]) * ((int)Math.Pow(255, (3 - i)));
                        for (int j = i + 1; j < 4; j++ )
                        {
                            diff += (left[j] - right[j]) * ((int)Math.Pow(255, (3 - j)));
                        }
                        return diff;
                    }
                }

                return 0;
            }

            public static int[] IPStringToDigits(String ip)
            {
                String[] parts = ip.Split('.');
                int[] digits = { Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3]) };
                return digits;
            }

            protected static String IncrementIP(String ip)
            {
                int[] digits = IPStringToDigits(ip);

                for (int i = 3; i >= 0; i--)
                {
                    digits[i]++;
                    if (digits[i] >= 255)
                    {
                        digits[i] = 2;
                    }
                    else
                    {
                        break;
                    }
                }

                return String.Format("{0}.{1}.{2}.{3}", digits[0], digits[1], digits[2], digits[3]);
            }
        }

        public static List<String> GetRangeOfFreeIPs(String subnet, int count, String minIp = "", String maxIp = "")
        {
            List<String> ips = new List<string>();
            List<String> existingClients = findDhcpClients(AcademicDHCPServer, subnet).Select(client => client.ip).ToList();

            List<ipaddr> addresses = new List<ipaddr>();
            ipaddr dotone = (new ipaddr(subnet)).next();
            addresses.Add(dotone);
            foreach (String ipstr in existingClients)
            {
                addresses.Add(new ipaddr(ipstr));
            }

            addresses.Sort();

            if (!minIp.Equals(""))
            {
                ipaddr min = new ipaddr(minIp);
                List<ipaddr> cutList = new List<ipaddr>();
                foreach (ipaddr addr in addresses)
                {
                    if (addr.CompareTo(min) >= 0)
                        cutList.Add(addr);
                }

                addresses = cutList;
            }

            if (!maxIp.Equals(""))
            {
                ipaddr max = new ipaddr(maxIp);
                List<ipaddr> cutList = new List<ipaddr>();
                foreach (ipaddr addr in addresses)
                {
                    if (addr.CompareTo(max) <= 0)
                        cutList.Add(addr);
                }

                addresses = cutList;
            }


            foreach (ipaddr addr in addresses)
            {
                ipaddr next = addr.next();
                while(!addresses.Contains(next) && count > 0)
                {
                    ips.Add(next.ip);
                    count--;
                    
                    next = next.next();
                } 
                if (count <= 0)
                    break;
            }

            return ips;
        }

        public static String GetNextFreeIP(String subnet, String minIp = "", String maxIp = "")
        {
            String ip = "0.0.0.0";
            List<String> existingClients = findDhcpClients(AcademicDHCPServer, subnet).Select(client => client.ip).ToList();

            if (!minIp.Equals("") && !existingClients.Contains(minIp))
                return minIp;

            List<ipaddr> addresses = new List<ipaddr>();
            foreach(String ipstr in existingClients)
            {
                addresses.Add(new ipaddr(ipstr));
            }

            addresses.Sort();

            if(!minIp.Equals(""))
            {
                ipaddr min = new ipaddr(minIp);
                List<ipaddr> cutList = new List<ipaddr>();
                foreach(ipaddr addr in addresses)
                {
                    if (addr.CompareTo(min) >= 0)
                        cutList.Add(addr);
                }

                addresses = cutList;
            }

            if (!maxIp.Equals(""))
            {
                ipaddr max = new ipaddr(maxIp);
                List<ipaddr> cutList = new List<ipaddr>();
                foreach (ipaddr addr in addresses)
                {
                    if (addr.CompareTo(max) <= 0)
                        cutList.Add(addr);
                }

                addresses = cutList;
            }


            foreach(ipaddr addr in addresses)
            {
                if (!addresses.Contains(addr.next())) 
                    return addr.next().ip;
            }

            return ip;
        }

        

        #region Code taken from http://www.ianatkinson.net/computing/dhcpcsharp.htm
        // c# class for processed clients  

        public class dhcpClient
        {
            public string hostname { get; set; }
            public string description { get; set; }
            public string ip { get; set; }
            public string mac { get; set; }
        }

        // structs for use with call to unmanaged code  

        [StructLayout(LayoutKind.Sequential)]
        public struct DHCP_CLIENT_INFO_ARRAY
        {
            public uint NumElements;
            public IntPtr Clients;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DHCP_CLIENT_UID
        {
            public uint DataLength;
            public IntPtr Data;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DHCP_CLIENT_INFO
        {
            public uint ip;
            public uint subnet;

            public DHCP_CLIENT_UID mac;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string ClientName;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string ClientComment;
        }

        public static List<dhcpClient> findDhcpClients(string server, string subnet)
        {
            // set up container for processed clients  

            List<dhcpClient> foundClients = new List<dhcpClient>();

            // make call to unmanaged code  

            uint parsedMask = StringIPAddressToUInt32(subnet);
            uint resumeHandle = 0;
            uint numClientsRead = 0;
            uint totalClients = 0;

            IntPtr info_array_ptr;

            uint response = DhcpEnumSubnetClients(
                server,
                parsedMask,
                ref resumeHandle,
                65536,
                out info_array_ptr,
                ref numClientsRead,
                ref totalClients
                );

            // set up client array casted to a DHCP_CLIENT_INFO_ARRAY  
            // using the pointer from the response object above  

            DHCP_CLIENT_INFO_ARRAY rawClients =
                (DHCP_CLIENT_INFO_ARRAY)Marshal.PtrToStructure(info_array_ptr, typeof(DHCP_CLIENT_INFO_ARRAY));

            // loop through the clients structure inside rawClients   
            // adding to the dchpClient collection  

            IntPtr current = rawClients.Clients;

            for (int i = 0; i < (int)rawClients.NumElements; i++)
            {
                // 1. Create machine object using the struct  

                DHCP_CLIENT_INFO rawMachine =
                    (DHCP_CLIENT_INFO)Marshal.PtrToStructure(Marshal.ReadIntPtr(current), typeof(DHCP_CLIENT_INFO));

                // 2. create new C# dhcpClient object and add to the   
                // collection (for hassle-free use elsewhere!!)  

                dhcpClient thisClient = new dhcpClient();

                thisClient.ip = UInt32IPAddressToString(rawMachine.ip);

                thisClient.hostname = rawMachine.ClientName;

                thisClient.description = rawMachine.ClientComment;

                thisClient.mac = String.Format("{0:x2}{1:x2}.{2:x2}{3:x2}.{4:x2}{5:x2}",
                    Marshal.ReadByte(rawMachine.mac.Data),
                    Marshal.ReadByte(rawMachine.mac.Data, 1),
                    Marshal.ReadByte(rawMachine.mac.Data, 2),
                    Marshal.ReadByte(rawMachine.mac.Data, 3),
                    Marshal.ReadByte(rawMachine.mac.Data, 4),
                    Marshal.ReadByte(rawMachine.mac.Data, 5));

                foundClients.Add(thisClient);

                // 3. move pointer to next machine  

                current = (IntPtr)((int)current + (int)Marshal.SizeOf(typeof(IntPtr)));
            }

            return foundClients;
        }

        public static uint StringIPAddressToUInt32(string ip)
        {
            // convert string IP to uint IP e.g. "1.2.3.4" -> 16909060  

            IPAddress i = System.Net.IPAddress.Parse(ip);
            byte[] ipByteArray = i.GetAddressBytes();

            uint ipUint = (uint)ipByteArray[0] << 24;
            ipUint += (uint)ipByteArray[1] << 16;
            ipUint += (uint)ipByteArray[2] << 8;
            ipUint += (uint)ipByteArray[3];

            return ipUint;
        }

        public static string UInt32IPAddressToString(uint ip)
        {
            // convert uint IP to string IP e.g. 16909060 -> "1.2.3.4"  

            IPAddress i = new IPAddress(ip);
            string[] ipArray = i.ToString().Split('.');

            return ipArray[3] + "." + ipArray[2] + "." + ipArray[1] + "." + ipArray[0];
        }

        [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint DhcpEnumSubnetClients(
                string ServerIpAddress,
                uint SubnetAddress,
            ref uint ResumeHandle,
                uint PreferredMaximum,
            out IntPtr ClientInfo,
            ref uint ElementsRead,
            ref uint ElementsTotal
        );
        #endregion
    }
}
