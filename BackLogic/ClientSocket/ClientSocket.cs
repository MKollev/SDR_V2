using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BackLogic.Model;


// State object for receiving data from remote device.
public class StateObject
{
    // Client socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 512;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    public StringBuilder sb = new StringBuilder();
}



public class AsynchronousClient : IDisposable
{
   
    private int Sth = 0;
    private List<FreqDataViewModel> Data = new List<FreqDataViewModel>(); 
    // The port number for the remote device.


    // ManualResetEvent instances signal completion.
    private static ManualResetEvent connectDone =
        new ManualResetEvent(false);
    private static ManualResetEvent sendDone =
        new ManualResetEvent(false);
    private static ManualResetEvent receiveDone =
        new ManualResetEvent(false);

    // The response from the remote device.
    private static String response = String.Empty;

    public async Task<List<FreqDataViewModel>> StartClient(SocketDataViewModel connectData)
    {
      
        // Debug.WriteLine("I AM ALIVE WITHOUT YOU KNOWING");
        //CombinedGraph.MainWindow.DrawingThread.Join();
        receiveDone.Reset();

        // Connect to a remote device.
        try
        {
            // Establish the remote endpoint for the socket.
            // The name of the 
            // remote device is "host.contoso.com".
            if (connectData.ServerAdress == null && connectData.Port == null && connectData.DnsServer == null)
            {
                connectData.DnsServer = Dns.GetHostName();
                IPHostEntry ipHostInfo = Dns.GetHostEntry(connectData.DnsServer);
                connectData.ServerAdress = ipHostInfo.AddressList[2].ToString();
                connectData.Port = 11000.ToString();
            }
            int port = Convert.ToInt32(connectData.Port);

            IPAddress ipAddress = IPAddress.Parse(connectData.ServerAdress);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            // Create a TCP/IP socket.
            Socket client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            // Connect to the remote endpoint.
            client.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();

            Send(client, "<EOF>");
            sendDone.WaitOne();
            // Send test data to the remote device.


            // Receive the response from the remote device.



            Receive(client);
            receiveDone.WaitOne();

            const char token = ',';
            string[] Received = response.Split(token);
            
           
           
                for (int j = 0; j < Received.Length; j += 2)
                {
                    Data.Add((new FreqDataViewModel
                    {
                        Ampl = (float) Convert.ToDouble(Received[j]),
                        Freq = Convert.ToDecimal(Received[j + 1])
                    }));
                }
            
                                                     

            // Release the socket\.
            client.Shutdown(SocketShutdown.Both);
            client.Close();


        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        //return Data;
        return Data;
    }

    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);

            // Console.WriteLine("Socket connected to {0}",
            //  client.RemoteEndPoint.ToString());

            // Signal that the connection has been made.
            connectDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private async Task Receive(Socket client)
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {

        try
        {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                string Ampl = Convert.ToString(state.sb);
                // Get the rest of the data.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            else
            {
                // All the data has arrived; put it in response.
                if (state.sb.Length > 1)
                {
                    response = state.sb.ToString();
                }
                // Signal that all bytes have been received.
                receiveDone.Set();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void Send(Socket client, String data)
    {
        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.
        client.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), client);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = client.EndSend(ar);
            // Console.WriteLine("Sent {0} bytes to server.", bytesSent);

            // Signal that all bytes have been sent.
            sendDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }


    public void Dispose()
    {
        
    }
}
