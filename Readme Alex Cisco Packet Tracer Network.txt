
Medium Difficulty Network Design for Cisco Packet Tracer
--------------------------------------------------------
1. Configure VLANs on all switches:
   - VLAN 10: Admin (192.168.1.0/24)
   - VLAN 20: Warehouse (192.168.2.0/24)
   - VLAN 30: Manufacturing (192.168.3.0/24)
2. Assign static IPs for servers and dynamic IPs for clients using DHCP.
3. Configure inter-VLAN routing on the Layer 3 switch.
4. Set up NAT and default routes on the router.
5. Test connectivity: Ping between VLANs, access ERP server, and verify internet access.


Medium Difficulty Network Design for Cisco Packet Tracer

Scenario Overview:
A small-scale professional network for an industrial company with the following locations:
1. Manufacturing Site
2. Admin Center (HR, Finance, and IT Support)

Network Objectives:
- Ensure proper VLAN segmentation for different departments.
- Provide firewall protection to secure internal and external traffic.
- Provide NAT for internet access.
- Enable inter-VLAN routing.
- Implement 802.1X authentication with a RADIUS server for identity-based access.
- Deploy a load balancer to distribute traffic efficiently.
- Basic network security.
- Provide DHCP and DNS services.

---

Network Topology

Devices:
1.  Core Layer:
	1 Layer 3 Switch (Cisco 3560) for inter-VLAN routing.
	1 Firewall Appliance (Cisco ASA) for traffic filtering and security.
	1 Load Balancer (Cisco CSM or software-based) for server traffic distribution.

2. Access Layer:
   -2 Layer 2 Switches (Cisco 2960) for VLAN segmentation and 802.1X authentication:
     - Manufacturing VLAN.
     - Admin VLAN.

3. Edge Devices:
    	1 Router (Cisco 2911) for internet access with NAT.
    	ISP cloud for internet connectivity.
 	Firewall (Cisco ASA) between the router and the internal network.

4. Servers:
	1 SAP ERP Server (Admin VLAN)
	1 DHCP/DNS Server (Admin VLAN)
	1 RADIUS Server (Admin VLAN, used for 802.1X authentication)
	1 Load Balancer (Admin VLAN)

5. End Devices:
   - PCs and Printers for each VLAN.

---

IP Addressing
- Subnet Mask: 255.255.255.0 (/24)
- IP Ranges:
  - Admin VLAN: 192.168.1.0/24
  - Firewall/Internal NAT: 192.168.4.0/24
  - Manufacturing VLAN: 192.168.3.0/24
  - Router-WAN: 203.0.113.0/30

---

Detailed Configuration Steps

1. Layer 2 Switch Configuration
- Create VLANs:
  - VLAN 10: Admin
  - VLAN 20: Warehouse
  - VLAN 30: Manufacturing

- Assign VLANs to Ports:
  ```
  enable
  configure terminal
  vlan 10
  name Admin
  vlan 20
  name Warehouse
  vlan 30
  name Manufacturing

  interface range fa0/1-10
  switchport mode access
  switchport access vlan 10

  interface range fa0/11-20
  switchport mode access
  switchport access vlan 20

  interface range fa0/21-24
  switchport mode access
  switchport access vlan 30
  end
  ```

- Trunk Ports for Interconnection:
  ```
  interface fa0/24
  switchport mode trunk
  end
  ```

2. Layer 3 Switch Configuration
- Inter-VLAN Routing:
  ```
  enable
  configure terminal
  interface vlan 10
  ip address 192.168.1.1 255.255.255.0
  no shutdown

  interface vlan 20
  ip address 192.168.2.1 255.255.255.0
  no shutdown

  interface vlan 30
  ip address 192.168.3.1 255.255.255.0
  no shutdown

  ip routing
  end
  ```

- Default Route to Router:
  ```
  ip route 0.0.0.0 0.0.0.0 192.168.4.1
  ```

3. Router Configuration
- Configure WAN and LAN Interfaces:
  ```
  enable
  configure terminal
  interface g0/0
  ip address 203.0.113.1 255.255.255.252
  no shutdown

  interface g0/1
  ip address 192.168.4.1 255.255.255.0
  no shutdown
  ```

- **NAT Configuration**:
  ```
  ip nat inside source list 1 interface g0/0 overload

  interface g0/1
  ip nat inside

  interface g0/0
  ip nat outside

  access-list 1 permit 192.168.0.0 0.0.255.255
  ```

- Default Route to ISP:
  ```
  ip route 0.0.0.0 0.0.0.0 203.0.113.2
  ```

4. SAP ERP Server Configuration
- Assign Static IP:
  - Admin VLAN: 192.168.1.100

- ERP Service Setup:
  - Verify connection to 192.168.1.1 (Layer 3 Switch).
  - Test connectivity from other VLANs.

5. DHCP Server Configuration
- DHCP Pools:
  ```
  ip dhcp excluded-address 192.168.1.1 192.168.1.50
  ip dhcp excluded-address 192.168.2.1 192.168.2.50
  ip dhcp excluded-address 192.168.3.1 192.168.3.50

  ip dhcp pool Admin
  network 192.168.1.0 255.255.255.0
  default-router 192.168.1.1
  dns-server 192.168.1.10

  ip dhcp pool Warehouse
  network 192.168.2.0 255.255.255.0
  default-router 192.168.2.1
  dns-server 192.168.1.10

  ip dhcp pool Manufacturing
  network 192.168.3.0 255.255.255.0
  default-router 192.168.3.1
  dns-server 192.168.1.10
  ```

6. Basic Security
- ACL to Restrict Telnet Traffic:
  ```
  access-list 101 deny tcp any any eq 23
  access-list 101 permit ip any any

  interface g0/1
  ip access-group 101 in
  ```

7. Testing
- Verify connectivity between VLANs with `ping`.
- Test NAT by accessing external websites.
- Ensure ERP server is accessible from all VLANs.
- Verify DHCP assignments for dynamic clients.
- 802.1X Authentication Test:
	Attempt to connect an unauthorized device → Access should be denied.
- Firewall Functionality:
	Try external access to ERP server → Should be allowed only on HTTPS (443).
	Try SSH access from external → Should be blocked.
-Load Balancer Test:
	Access ERP Server via Virtual IP → Requests should be evenly distributed.

---
