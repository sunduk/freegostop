using System;

/// <summary>
/// 프로토콜 정의.
/// 서버에서 클라이언트로 가는 패킷 : S -> C
/// 클라이언트에서 서버로 가는 패킷 : C -> S
/// </summary>
public enum PROTOCOL : short
{
	BEGIN = 0,

	// 시스템 프로토콜.
    LOCAL_SERVER_STARTED = 1,	// S -> C

	// 게임 프로토콜.
	READY_TO_START = 10,		// C -> S
	BEGIN_CARD_INFO = 11,		// S -> C
	DISTRIBUTED_ALL_CARDS = 12,	// C -> S

	SELECT_CARD_REQ = 13,		// C -> S
	SELECT_CARD_ACK = 14,		// S -> C

	// 플레이어가 두개의 카드 중 하나를 선택해야 하는 경우.
    //CHOICE_ONE_CARD = 15,		// S -> C   // 사용하지 않음.
	CHOOSE_CARD = 16,			// C -> S

    FLIP_BOMB_CARD_REQ = 17,	// C -> S

	FLIP_DECK_CARD_REQ = 18,	// C -> S
	FLIP_DECK_CARD_ACK = 19,	// S -> C

	TURN_RESULT = 20,			// S -> C

	ASK_GO_OR_STOP = 21,		// S -> C
	ANSWER_GO_OR_STOP = 22,		// C -> S
	NOTIFY_GO_COUNT = 23,		// S -> C

	UPDATE_PLAYER_STATISTICS = 24,	// S -> C

	ASK_KOOKJIN_TO_PEE = 25,		// S -> C
	ANSWER_KOOKJIN_TO_PEE = 26,		// C -> S

	MOVE_KOOKJIN_TO_PEE = 27,		// S -> C

	GAME_RESULT = 28,				// S -> C

	PLAYER_ORDER_RESULT = 29,		// S -> C

	START_TURN = 98,
	TURN_END = 100,

	END
}


// 플레이어가 낸 카드에 대한 결과.
public enum PLAYER_SELECT_CARD_RESULT : byte
{
	// 완료.
	COMPLETED,

	// 카드 한장 선택해야 함(플레이어가 낸 경우).
	CHOICE_ONE_CARD_FROM_PLAYER,

	// 카드 한장 선택해야 함(덱에서 뒤집은 경우).
	CHOICE_ONE_CARD_FROM_DECK,

	// 에러.
	ERROR_INVALID_CARD
}


// 카드 이벤트 정의.
public enum CARD_EVENT_TYPE : byte
{
	NONE,

	// 쪽.
	KISS,

	// 뻑.
	PPUK,

	// 따닥.
	DDADAK,

	// 폭탄.
	BOMB,

	// 싹쓸이.
	CLEAN,

	// 뻑 먹기.
	EAT_PPUK,

	// 자뻑.
	SELF_EAT_PPUK,

	// 흔들기.
	SHAKING
}


public enum TURN_RESULT_TYPE : byte
{
    // 일반 카드를 낸 후의 결과.
    RESULT_OF_NORMAL_CARD,

    // 폭탄 카드를 낸 후의 결과.
    RESULT_OF_BOMB_CARD
}
