using System;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

public enum PLAYER_TYPE : byte
{
    HUMAN,
    AI
}

public class CPlayer
{
    public delegate void SendFn(CPacket msg);

    SendFn send_function;
    public byte player_index { get; private set; }
    public CPlayerAgent agent { get; private set; }

    PLAYER_TYPE player_type;
    CAIPlayer ai_logic;

    public CPlayer(byte player_index, PLAYER_TYPE player_type, SendFn send_function, CLocalServer local_server)
    {
        this.player_index = player_index;
		this.agent = new CPlayerAgent(player_index);
        this.player_type = player_type;

        switch (this.player_type)
        {
            case PLAYER_TYPE.HUMAN:
                this.send_function = send_function;
                break;

            case PLAYER_TYPE.AI:
                this.ai_logic = new CAIPlayer(local_server);
                this.send_function = this.ai_logic.send;
                break;
        }
    }


    public void send(CPacket msg)
    {
        this.send_function(msg);
    }


	public void reset()
	{
		if (this.ai_logic != null)
		{
			this.ai_logic.reset();
		}
	}


    public bool is_autoplayer()
    {
        return this.player_type == PLAYER_TYPE.AI;
    }
}
