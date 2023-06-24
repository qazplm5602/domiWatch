<?php
    $domiHeader = "다운로드";
    include "_headTag.php";
?>
<body>
    <link rel="stylesheet" href="./css/download_box.css">
    <link rel="stylesheet" href="./css/warning_alert.css">
    <link rel="stylesheet" href="./css/bannder.css">

    <?php include_once "_header.php"; ?>

    <!-- 뒤에 배경이 있는 박스으 -->
    <div class="download-box">
        <div class="black-back"></div>

        <!-- 내용s -->
        <section class="download-content">
            <div class="title">★ 100% ※무료 다운로드 ★</div>
            <a href="https://domi.kr/img/logo.png" download>
                <button class="download-button">다운로드</button>
            </a>
        </section>
    </div>

    <!-- 주의 사항 -->
    <div class="alert-box">
        <div class="alert-title">주의사항</div>

        <li>게임을 플레이 하려면 인터넷이 연결되어 있어야 합니다.</li>
        <li>건전한 게임 문화를 위해 닉네임, 채팅 등 입력칸에 욕설, 폭언 등 하지 않도록 해주세요.</li>
        <li>게임서버에 DDOS 공격 등 고의적으로 트래픽을 올리는 행위나, 취약점 공격을 금지합니다.</li>
        <li>Client에서 로그인 시 기기를 식별하기 위해 IP 주소와 HWID를 수집합니다.</li>
    </div>

    <a class="bannder-domi" href="./Downloads/haejeohaejeok.zip" download="">
        <img src="./assets/db6a86a9ef11a029.gif">
    </a>
</body>
</html>