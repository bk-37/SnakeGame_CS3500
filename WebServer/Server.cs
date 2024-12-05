// <copyright file="Server.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
/// <authors>
/// Brian Keller & Wyatt Young
/// </authors>
/// <version>
/// November 3rd, 2024
/// </version>
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace CS3500.Networking;

/// <summary>
///   Represents a server task that waits for connections on a given
///   port and calls the provided delegate when a connection is made.
/// </summary>
public static class Server
{
    /// <summary>
    /// private static member variable for TCP listener initialized in StartServer. Null if server not started
    /// </summary>
    private static TcpListener? _listener;

    /// <summary>
    ///   Wait on a TcpListener for new connections. Alert the main program
    ///   via a callback (delegate) mechanism.
    /// </summary>
    /// <param name="handleConnect">
    ///   Handler for what the user wants to do when a connection is made.
    ///   This should be run asynchronously via a new thread.
    /// </param>
    /// <param name="port"> The port (e.g., 11000) to listen on. </param>
    public static void StartServer( Action<NetworkConnection> handleConnect, int port )
    {
        // create and start new TCP listener and initialize list of clients
        _listener = new(IPAddress.Any, port);
        _listener.Start();
        // asynchronously accept connections into new threads
        while (true) 
        {
            TcpClient client = _listener.AcceptTcpClient();
            //clients.Add( client );
            Console.WriteLine("Accepted new connection");
            new Thread(() => handleConnect(new NetworkConnection(client))).Start();
        }
    }
}
