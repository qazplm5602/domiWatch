<?php
    $domiHeader = "서버 업타임";
    include "_headTag.php";
?>
<body>
    <link rel="stylesheet" href="./css/domiserver.css">
    <style>
        body {
            background: #131313;
        }
    </style>
    
    <?php include_once "_header.php"; ?>

    <!-- 서버 정보 -->
    <div class="server-title">서버 정보</div>
    <section class="server-box" domi-endpoint="ggm.domi.kr" domi-uri="domiWatch/api/status">
        <img class="server-icon domiReverse" src="./assets/157359.png">
        <section class="half-list">
            <div>도미서버 - 1 <span class="status offline">불러오는중...</span></div>
            <div>Ping: -- <span>--명 접속중</span></div>
        </section>
    </section>
    <section class="server-box">
        <img class="server-icon domiReverse" src="./assets/157359.png">
        <section class="half-list">
            <div>도미서버 - 2 <span class="status offline">오프라인</span></div>
            <div>Ping: -- <span>--명 접속중</span></div>
        </section>
    </section>
    <section class="server-box">
        <img class="server-icon domiReverse" src="./assets/157359.png">
        <section class="half-list">
            <div>도미서버 - 비상용 <span class="status offline">오프라인</span></div>
            <div>Ping: -- <span>--명 접속중</span></div>
        </section>
    </section>
</body>
</html>