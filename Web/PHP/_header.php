<?php
    $domi_LIST = array(
        "메인" => "",
        "다운로드" => "download",
        "서버 업타임" => "uptime",
    );
?>

<!-- Header (이건 php로 incloud 할꺼임) -->
<header class="header-main">
    <img class="header-logo" src="./assets/domiLogo.png">
    <section class="header-menus">
        <?php foreach ($domi_LIST as $title => $uri) { ?>
            <a href="./<?php echo $uri ?>" class="<?php if ($domiHeader == $title) echo "active"; ?>"><?php echo $title; ?></a>
        <?php } ?>
    </section>
    <button class="header-Download">
        <div class="BackGround_Domi"></div>
        <div class="domi-marker"></div>
        <div class="download-text">다운로드</div>
    </button>
</header>

<!-- 깃허브 -->
<link rel="stylesheet" href="./css/github.css">
<div class="git-main">
    <a href="https://github.com/qazplm5602/domiWatch">
        <img src="./assets/github.svg">
    </a>
</div>