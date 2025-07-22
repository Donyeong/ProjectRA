1. Client1(호스트)에서 방을 열 때
호스트는 서버와 클라이언트가 동시에 동작합니다.
Client1(호스트)에서 실행되는 콜백 순서:
1.	OnStartHost()
(호스트 모드에서만 호출, 서버+클라이언트 모두 시작)
2.	OnStartServer()
(서버 시작)
3.	OnStartClient()
(클라이언트 시작)
4.	OnClientConnect()
(호스트의 클라이언트가 서버에 연결됨)
5.	OnServerConnect(conn)
(서버에 클라이언트(자기 자신)가 연결됨)
6.	OnServerAddPlayer(conn)
(서버에서 플레이어 오브젝트 생성)
---
2. Client2가 접속할 때
서버(호스트)에서 실행되는 콜백:
1.	OnServerConnect(conn)
(새 클라이언트가 서버에 연결됨)
2.	OnServerAddPlayer(conn)
(서버에서 해당 클라이언트의 플레이어 오브젝트 생성)
Client2(클라이언트)에서 실행되는 콜백:
1.	OnStartClient()
(클라이언트 시작)
2.	OnClientConnect()
(서버에 연결됨)
---
전체 요약
•	호스트(방장)에서 방을 열 때:
OnStartHost → OnStartServer → OnStartClient → OnClientConnect(호스트) → OnServerConnect(호스트) → OnServerAddPlayer(호스트)
•	클라이언트가 접속할 때:
(서버) OnServerConnect → OnServerAddPlayer
(클라이언트) OnStartClient → OnClientConnect
---