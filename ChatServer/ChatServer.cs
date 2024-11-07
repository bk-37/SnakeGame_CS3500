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
            // first message is clients name
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
                    clients[name] = connection;
                    break;
                }
            }
            while (true)
            {
                var message = connection.ReadLine();
                //send message prepended with name
                if (message != null)
                    SendAll($"({name}) {message}");
                else
                    throw new IOException();
            }
        }
        catch (IOException)
        {
            //remove from dictionary
            clients.Remove(name);
            //send message saying they left
            SendAll($"{name} has left the server.");
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
        foreach (var client in clients.Values)
        {
            try
            {
                client.Send(message);
            }
            catch { }
        }
    }
}
/// <summary>
/// Exception for when a client disconnects
/// </summary>
public class DisconnectException : Exception
{
    public DisconnectException(string message) : base(message) { }
}