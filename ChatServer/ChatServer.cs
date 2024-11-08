// <copyright file="ChatServer.cs" company="UoU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using CS3500.Networking;
using System.Text;

namespace CS3500.Chatting;

/// <summary>
///   A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{
    /// <summary>
    /// Private member list of clients currently on server. Null if no clients
    /// </summary>
    private static Dictionary<string, NetworkConnection> clients = new();
    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    /// <returns> A Task. Not really used. </returns>
    private static void Main(string[] args)
    {
        Server.StartServer(HandleConnect, 11_000);
        Console.Read(); // don't stop the program.
    }


    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    ///
    private static void HandleConnect(NetworkConnection connection)
    {
        // handle all messages until disconnect.
        string? name = null;
        try
        {
            // first loop till name is valid 
            while (true)
            {
                name = connection.ReadLine();
                if (string.IsNullOrEmpty(name) || clients.ContainsKey(name))
                {
                    connection.Send("Invalid or already used name!");
                    continue;
                }
                // add client and name to dict of clients
                else
                {
                    lock (name)
                    {
                        clients[name] = connection;
                    }
                    break;
                }
            }
            // resume main chat room loop
            while (true)
            {
                var message = connection.ReadLine();
                // check for null message from disconnect button
                if (message != null)
                    lock (message)
                    {
                        // send message prepended with name
                        SendAll($"({name}) {message}");
                    }
                else
                    throw new IOException();
            }
        }
        catch (IOException)
        {
            lock (name)
            {
                //remove from dictionary
                clients.Remove(name);
                //send message saying they left
                SendAll($"{name} has left the server.");
            }
            //Disconnect the connection
            connection.Disconnect();
        }
    }
    /// <summary>
    /// private helper method to send the message to all clients in the dictionary
    /// </summary>
    /// <param name="message"> message to send to clients</param>
    private static void SendAll(string message)
    {
        // loop through clients
        foreach (var client in clients.Values)
        {
            try
            {
                // send message 
                client.Send(message);
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}