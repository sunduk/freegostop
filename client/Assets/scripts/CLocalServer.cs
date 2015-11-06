using System;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

public class CLocalServer
{
    // 네트워크 메시지 수신시 호출되는 델리게이트. 어플리케이션에서 콜백 매소드를 설정하여 사용한다.
    public delegate void MessageHandler(CPacket msg);
    public MessageHandler appcallback_on_message;

    CGameRoom game_room;
    List<CPlayer> players;

    public CLocalServer()
    {
        this.players = new List<CPlayer>();
        this.players.Add(new CPlayer(0, PLAYER_TYPE.HUMAN, this.send, this));
        this.players.Add(new CPlayer(1, PLAYER_TYPE.AI, null, this));

        this.game_room = new CGameRoom(this);
        for (int i = 0; i < this.players.Count; ++i)
        {
            this.game_room.add_player(this.players[i]);
        }
    }


    public void start_localserver()
    {
        for (int i = 0; i < this.players.Count; ++i)
        {
            CPacket msg = CPacket.create((short)PROTOCOL.LOCAL_SERVER_STARTED);
            CPacket clone = CLocalServer.pre_send(msg);
            this.players[i].send(clone);
        }
    }


    public static CPacket pre_send(CPacket msg)
    {
        msg.record_size();

        CPacket clone = CPacket.create(msg.protocol_id);
        clone.overwrite(msg.buffer, 0);
        clone.pop_int16();		// LocalServer환경에서는 size값이 필요 없으므로 2바이트를 버린다.

        return clone;
    }


    void send(CPacket msg)
    {
        CPacket clone = CLocalServer.pre_send(msg);
        CPacket.destroy(msg);
        this.appcallback_on_message(clone);
    }


    public void on_receive_from_client(CPacket msg)
    {
        msg.record_size();
		CPacket clone = CPacket.create(msg.protocol_id);
		clone.overwrite(msg.buffer, 0);
		clone.pop_int16();		// LocalServer환경에서는 size값이 필요 없으므로 2바이트를 버린다.
        this.game_room.on_receive(this.players[0], clone);
    }


    public void on_receive_from_ai(CPacket msg)
    {
        msg.record_size();
        CPacket clone = CPacket.create(msg.protocol_id);
        clone.overwrite(msg.buffer, 0);
        clone.pop_int16();		// LocalServer환경에서는 size값이 필요 없으므로 2바이트를 버린다.
        this.game_room.on_receive(this.players[1], clone);
    }
}
