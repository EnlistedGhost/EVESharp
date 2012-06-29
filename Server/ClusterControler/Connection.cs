﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

using Common.Packets;
using Common.Network;
using Common.Game;
using Common;

using Marshal;

namespace EVESharp.ClusterControler
{
    public class Connection
    {
        private AsyncCallback recvAsync;
        private AsyncCallback sendAsync;
        private StreamPacketizer packetizer = new StreamPacketizer();

        public Connection(Socket sock)
        {
            Socket = new TCPSocket(sock, false);

            // Declare handlers
            recvAsync = new AsyncCallback(ReceiveAuthAsync);
            sendAsync = new AsyncCallback(SendAsync);

            // Session data
            Session = new Session();

            StageEnded = false;

            // Send LowLevel version exchange
            SendLowLevelVersionExchange();

            AsyncState state = new AsyncState();

            // Start receiving
            Socket.Socket.BeginReceive(state.buffer, 0, 8192, SocketFlags.None, recvAsync, state);
        }

        public void ReceiveAuthAsync(IAsyncResult ar)
        {
            try
            {
                AsyncState state = (AsyncState)(ar.AsyncState);

                int bytes = Socket.Socket.EndReceive(ar);

                packetizer.QueuePackets(state.buffer, bytes);
                int p = packetizer.ProcessPackets();

                for (int i = 0; i < p; i++)
                {
                    try
                    {
                        byte[] packet = packetizer.PopItem();

                        PyObject obj = Unmarshal.Process<PyObject>(packet);

                        if (obj != null)
                        {
                            PyObject result = TCPHandler.ProcessAuth(obj, this);

                            if (result != null)
                            {
                                Send(result);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Log.Error("Connection", ex.ToString());
                    }
                }

                if (StageEnded == true)
                {
                    if (Type == ConnectionType.Node)
                    {
                        recvAsync = new AsyncCallback(ReceiveNodeAsync);
                    }
                    else if (Type == ConnectionType.Client)
                    {
                        recvAsync = new AsyncCallback(ReceiveClientAsync);
                    }
                }

                // Continue receiving data
                Socket.Socket.BeginReceive(state.buffer, 0, 8192, SocketFlags.None, recvAsync, state);
            }
            catch (ObjectDisposedException)
            {
                Log.Debug("Connection", "Disconnected");
                ConnectionManager.RemoveConnection(this);
            }
            catch (SocketException)
            {
                Log.Debug("Connection", "Disconnected");
                ConnectionManager.RemoveConnection(this);
            }
            catch (Exception ex)
            {
                Log.Error("Connection", "Caught unhandled exception: " + ex.ToString());
            }
        }

        public void ReceiveNodeAsync(IAsyncResult ar)
        {
            try
            {
                AsyncState state = (AsyncState)(ar.AsyncState);

                int bytes = Socket.Socket.EndReceive(ar);

                packetizer.QueuePackets(state.buffer, bytes);
                int p = packetizer.ProcessPackets();

                for (int i = 0; i < p; i++)
                {
                    try
                    {
                        byte[] packet = packetizer.PopItem();

                        PyObject obj = Unmarshal.Process<PyObject>(packet);

                        if ((obj is PyObjectData) == false)
                        {
                            Log.Debug("Node", "Non-valid node packet. Dropping");
                            continue;
                        }

                        PyObjectData item = obj as PyObjectData;

                        if (item.Name == "macho.CallRsp")
                        {
                            PyPacket final = new PyPacket();

                            if (final.Decode(item) == false)
                            {
                                Log.Error("Node", "Cannot decode packet");
                                continue;
                            }

                            // We do not need to care about the destination
                            ConnectionManager.NotifyConnection((int)(final.userID), obj);

                            // TODO: Handle Broadcast packets
                        }
                        else
                        {
                            Log.Error("Node", string.Format("Wrong packet name: {0}", item.Name));
                        }
                    }
                    catch(Exception ex)
                    {
                        Log.Error("Node", ex.ToString());
                    }
                }

                Socket.Socket.BeginReceive(state.buffer, 0, 8192, SocketFlags.None, recvAsync, state);
            }
            catch (ObjectDisposedException)
            {
                Log.Debug("Node", "Disconnected");
                ConnectionManager.RemoveConnection(this);
            }
            catch (SocketException)
            {
                Log.Debug("Node", "Disconnected");
                ConnectionManager.RemoveConnection(this);
            }
            catch(Exception ex)
            {
                Log.Error("Node", "Caught unhandled exception: " + ex.ToString());
            }
        }

        public void ReceiveClientAsync(IAsyncResult ar)
        {
            try
            {
                AsyncState state = (AsyncState)(ar.AsyncState);

                int bytes = Socket.Socket.EndReceive(ar);

                packetizer.QueuePackets(state.buffer, bytes);
                int p = packetizer.ProcessPackets();

                for (int i = 0; i < p; i++)
                {
                    byte[] actual = packetizer.PopItem();
                    PyObject obj = Unmarshal.Process<PyObject>(actual);

                    if (obj == null)
                    {
                        continue;
                    }

                    if (obj is PyObjectEx)
                    {
                        // PyException
                        Log.Error("Client", "Got exception from client");
                    }
                    else
                    {

                        PyPacket packet = new PyPacket();

                        if (packet.Decode(obj) == false)
                        {
                            Log.Error("Client", "Error decoding PyPacket");
                        }
                        else
                        {
                            if (packet.dest.type == PyAddress.AddrType.Node)
                            {
                                if (packet.source.type != PyAddress.AddrType.Client)
                                {
                                    Log.Error("Client", string.Format("Wrong source data, expected client but got {0}", packet.source.type));
                                }

                                Log.Warning("Client", PrettyPrinter.Print(packet.Encode()));

                                // Notify the node, be careful here, the client will be able to sent packets to game clients
                                ConnectionManager.NotifyConnection((int)(packet.dest.typeID), obj);
                            }
                        }
                    }
                }

                Socket.Socket.BeginReceive(state.buffer, 0, 8192, SocketFlags.None, recvAsync, state);
            }
            catch (ObjectDisposedException)
            {
                Log.Debug("Client", "Disconnected");
                ConnectionManager.RemoveConnection(this);
            }
            catch (SocketException)
            {
                Log.Debug("Client", "Disconnected");
                ConnectionManager.RemoveConnection(this);
            }
            catch (Exception ex)
            {
                Log.Error("Client", "Caught unhandled exception: " + ex.ToString());
            }
        }

        public void SendAsync(IAsyncResult ar)
        {
            int bytes = Socket.Socket.EndSend(ar);
        }

        public void Send(PyObject packet)
        {
            Send(Marshal.Marshal.Process(packet));
        }

        public void Send(byte[] data)
        {
            byte[] packet = new byte[data.Length + 4];

            Array.Copy(BitConverter.GetBytes(data.Length), packet, 4);
            Array.Copy(data, 0, packet, 4, data.Length);

            // Send data
            // Socket.Socket.BeginSend(packet, 0, packet.Length, SocketFlags.None, sendAsync, null);
            Socket.Send(packet); // Maybe no the best solution, but should be helpful
        }

        public void SendLowLevelVersionExchange()
        {
            Log.Debug("Client", "Sending LowLevelVersionExchange...");

            LowLevelVersionExchange data = new LowLevelVersionExchange();

            data.codename = Common.Constants.Game.codename;
            data.birthday = Common.Constants.Game.birthday;
            data.build = Common.Constants.Game.build;
            data.machoVersion = Common.Constants.Game.machoVersion;
            data.version = Common.Constants.Game.version;
            data.usercount = ConnectionManager.ClientsCount;
            data.region = Common.Constants.Game.region;

            Send(data.Encode(false));
        }

        public bool CheckLowLevelVersionExchange(PyTuple packet)
        {
            LowLevelVersionExchange data = new LowLevelVersionExchange();

            if (data.Decode(packet) == false)
            {
                Log.Error("Client", "Wrong LowLevelVersionExchange packet");
                return false;
            }

            if (data.birthday != Common.Constants.Game.birthday)
            {
                Log.Error("Client", "Wrong birthday in LowLevelVersionExchange");
                return false;
            }

            if (data.build != Common.Constants.Game.build)
            {
                Log.Error("Client", "Wrong build in LowLevelVersionExchange");
                return false;
            }

            if (data.codename != Common.Constants.Game.codename + "@" + Common.Constants.Game.region)
            {
                Log.Error("Client", "Wrong codename in LowLevelVersionExchange");
                return false;
            }

            if (data.machoVersion != Common.Constants.Game.machoVersion)
            {
                Log.Error("Client", "Wrong machoVersion in LowLevelVersionExchange");
                return false;
            }

            if (data.version != Common.Constants.Game.version)
            {
                Log.Error("Client", "Wrong version in LowLevelVersionExchange");
                return false;
            }

            if (data.isNode == true)
            {
                if (data.nodeIdentifier != "Node")
                {
                    Log.Error("Client", "Wrong node string in LowLevelVersionExchange");
                    return false;
                }

                Type = ConnectionType.Node;
            }
            else
            {
                Type = ConnectionType.Client;
            }

            return true;
        }

        public void EndConnection()
        {
            try
            {
                Socket.Socket.Shutdown(SocketShutdown.Both);
                Socket.Socket.Close();
            }
            catch (ObjectDisposedException)
            {
                Log.Debug("Client", "Trying to close a connection which is already closed");
            }
            catch (SocketException)
            {
                Log.Debug("Client", "Trying to close a connection which is already closed");
            }
            catch
            {

            }

            ConnectionManager.RemoveConnection(ClusterConnectionID);
        }

        public void SendSessionChange()
        {
            PyPacket sc = CreateEmptySessionChange();

            PyObject client = SetSessionChangeDestination(sc);
            PyObject node = SetSessionChangeDestination(sc, NodeID);

            if (sc != null)
            {
                Send(client);
                ConnectionManager.NotifyConnection(NodeID, node);
            }
        }

        public PyPacket CreateEmptySessionChange()
        {
            // Fill all the packet data, except the dest/source
            SessionChangeNotification scn = new SessionChangeNotification();
            scn.changes = Session.EncodeChanges();

            if (scn.changes.Dictionary.Count == 0)
            {
                // Nothing to do
                return null;
            }

            List<Connection> nodes = ConnectionManager.GetNodeList();

            // Add all the nodeIDs
            foreach (Connection node in nodes)
            {
                scn.nodesOfInterest.Items.Add(new PyInt(node.NodeID));
            }

            PyPacket p = new PyPacket();

            p.type_string = "macho.SessionChangeNotification";
            p.type = Macho.MachoNetMsg_Type.SESSIONCHANGENOTIFICATION;

            p.userID = (uint)ClusterConnectionID;

            p.payload = scn.Encode().As<PyTuple>();

            p.named_payload = new PyDict();
            p.named_payload.Set("channel", new PyString("sessionchange"));

            return p;
        }

        public PyObject SetSessionChangeDestination(PyPacket p)
        {
            p.source.type = PyAddress.AddrType.Node;
            p.source.typeID = (ulong)NodeID;
            p.source.callID = 0;

            p.dest.type = PyAddress.AddrType.Client;
            p.dest.typeID = (ulong)ClusterConnectionID;
            p.dest.callID = 0;

            return p.Encode();
        }

        public PyObject SetSessionChangeDestination(PyPacket p, int node)
        {
            // The session change info should never come from the client
            p.source.type = PyAddress.AddrType.Node;
            p.source.typeID = (ulong)1;
            p.source.callID = 0;

            p.dest.type = PyAddress.AddrType.Node;
            p.dest.typeID = (ulong)node;
            p.dest.callID = 0;

            return p.Encode();
        }

        public PyObject CreateSessionChange()
        {
            PyPacket p = CreateEmptySessionChange();

            if (p == null)
            {
                return null;
            }

            p.source.type = PyAddress.AddrType.Node;
            p.source.typeID = (ulong)NodeID;
            p.source.callID = 0;

            p.dest.type = PyAddress.AddrType.Client;
            p.dest.typeID = (ulong)ClusterConnectionID;
            p.dest.callID = 0;

            return p.Encode();
        }

        public PyObject CreateSessionChange(int nodeid)
        {
            PyPacket p = CreateEmptySessionChange();

            if (p == null)
            {
                return null;
            }

            // The session change info should never come from the client
            p.source.type = PyAddress.AddrType.Node;
            p.source.typeID = (ulong)1;
            p.source.callID = 0;

            p.dest.type = PyAddress.AddrType.Node;
            p.dest.typeID = (ulong)nodeid;
            p.dest.callID = 0;

            return p.Encode();
        }

        public void SendNodeChangeNotification()
        {
            if (Type != ConnectionType.Node)
            {
                return;
            }

            NodeInfo nodeInfo = new NodeInfo();

            nodeInfo.nodeID = NodeID;
            nodeInfo.solarSystems.Items.Add(new PyNone()); // None = All solar systems

            Send(nodeInfo.Encode());
        }

        public bool StageEnded
        {
            get;
            set;
        }

        public bool Banned
        {
            get;
            set;
        }

        public int Role
        {
            get;
            set;
        }

        public int AccountID
        {
            get;
            set;
        }

        public string Address
        {
            get
            {
                return Socket.GetAddress();
            }

            private set
            {

            }
        }

        public string LanguageID
        {
            get;
            set;
        }

        // This can have two meanings
        // When connection is a Node this is the nodeID(the same as ClusterConnectionID)
        // When connection is a Client, the node in which it is now
        public int NodeID
        {
            get;
            set;
        }

        public TCPSocket Socket
        {
            get;
            set;
        }

        public Session Session
        {
            get;
            set;
        }

        public ConnectionType Type
        {
            get;
            set;
        }

        // ID in the ConnectionManager list
        public int ClusterConnectionID
        {
            get;
            set;
        }
    }
}
