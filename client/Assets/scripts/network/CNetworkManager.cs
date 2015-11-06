using UnityEngine;
using System;
using System.Collections;
using FreeNet;
using FreeNetUnity;

public interface IMessageReceiver
{
    void on_recv(CPacket msg);
}

public class CNetworkManager : CSingletonMonobehaviour<CNetworkManager>
{
	CLocalServer gameserver;
	string received_msg;

	public IMessageReceiver message_receiver;


	void Awake()
	{
		this.received_msg = "";

        this.gameserver = new CLocalServer();
		this.gameserver.appcallback_on_message += on_message;
	}


    public void start_localserver()
    {
        this.gameserver.start_localserver();
    }


	void on_message(CPacket msg)
	{
		this.message_receiver.on_recv(msg);
        CPacket.destroy(msg);
	}


	public void send(CPacket msg)
	{
        this.gameserver.on_receive_from_client(msg);
		CPacket.destroy(msg);
	}
}
