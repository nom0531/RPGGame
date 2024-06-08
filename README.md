# RPGGAME
<img src="Portfolio/png/0_Title.png" width="60%"><br>

### 目次
---
1. 自己紹介
2. 紹介動画<br>
3. 作品概要<br>
4. 制作環境<br>
5. 技術紹介<br>
6. こだわり<br>
7. 使用素材<br>
8. 楽曲に関しまして<br>

### 1. 自己紹介
---
<dd>
河原電子ビジネス専門学校　ゲームクリエイター科3年<br>
野村 華生(のむら かなり)<br><br>
ゲーム、絵を描くこと、写真を撮ることが趣味です。
</dd>

### 2. 紹介動画
---
<dd>

[プレイ動画](https://youtu.be/Tf6d_NLJHek "https://youtu.be/Tf6d_NLJHek")
</dd>

### 3. 作品概要
---
<dd>
    <dd>
    プレイ人数<br>
    1人<br>
    </dd><br>
    <dd>
    ジャンル<br>
    RPG<br>
    </dd><br>
    <dd>
    対応ハード<br>
    Windows11<br>
    </dd><br>
</dd>

### 4. 制作環境
---
<dd>
    <dd>
    制作期間<br>
    2023年11月～2024年5月<br>
    </dd><br>
    <dd>
    制作人数<br>
    1人<br>
    </dd><br>
    <dd>
    使用言語<br>
    C#、HLSL<br>
    </dd><br>
    <dd>
    使用ツール<br>
    Unity2021.3.23f1<br>
    Visual Studio 2022<br>
    Visual Studio Code<br>
    Adobe Photoshop 2022<br>
    fork<br>
    Github<br>
    Github Large File Storage<br>
    </dd><br>
</dd>

### 5. 技術紹介
---
#### 5.1. エディタ拡張
<dd>
<img src="Portfolio/png/4_editor.png" width="60%"><br>
本作ではプレイヤーデータ、エネミーデータ等、膨大な量のデータを使用しています。<br>そのデータを簡単に、わかりやすく扱うためにエディタ拡張を追加しています。

<img src="Portfolio/png/4_editor2.png" width="60%"><br>
いくつかあるデータの中でも、エネミーデータが最も多く、データ入力、データ追加を考えると非常に効率が悪くなることが考えられるため、エディタ拡張を導入しました。<br><br>
データとして扱っているものすべてをエディタとして管理できるようにしたため、データを簡単に管理できるようになりました。
</dd>

#### 5.2. セーブデータの暗号化
<dd>
本作ではプレイヤーのステータス、スキル、ステージのクリア状況などをセーブデータで保存し、記録しています。<br>
しかし、暗号化を実装しない限り中身の改変が可能なため、暗号化を実装しました。<br>
</dd>

#### 5.3. Unitask
<dd>
ゲームオーバー、ゲームクリアなどの判定を行うほか、演出部分で一定時間待機する等の処理に使用しています。<br>
※黄色のラインの部分がUnitaskのコード<br>
<img src="Portfolio/png/4_Unitask.png" width="90%"><br>
▲ 演出の終了処理

<img src="Portfolio/png/4_Unitask2.png" width="90%"><br>
▲ ゲームの終了処理

</dd>

### 6. こだわり
---
<dd>

#### 6.1. 絵文字
テキストと同時にアイコンを表示したいと考え、絵文字を使用しました。<br>
<img src="Portfolio/png/5_emoji.png" width="30%"><br>
<img src="Portfolio/png/5_emoji2.png" width="30%"><br>

</dd>

<dd>

#### 6.2. アニメーション
<img src="Portfolio/gif/6_Animation.gif" width="60%"><br>
UIにこだわりたいと考え、ボタンを押したときなどのアニメーションを個別に追加しています。<br>
同一のAnimatorを使用しており、同じスクリプトをアタッチして設定するだけで制御できるようにしています。

作成する際には「何度も押したくなるかどうか？」「自分がアニメーションを見て違和感を感じないかどうか？」を重視して作成しています。

<img src="Portfolio/gif/6_Animation2.gif" width="60%"><br>
また、システムのセーブが行われた際の専用のアニメーションを追加しました。<br>
アニメーションイベントを使用し、回転が行われた瞬間に画像を差し替えて表裏があるように見せています。

スートが描かれた画像は乱数を設定して決定しており、毎回ランダムに切り替わるようにしています。

#### 6.2.1 タイムラインを使用したシームレスな遷移
<img src="Portfolio/gif/6_Timeline.gif" width="60%"><br>

Unityの機能であるTimelineについての理解を深めたいと思い、<br>アウトゲームの背景の遷移の部分でTimelineを活用して、シームレスな遷移を実装しました。

<img src="Portfolio/png/6_timeline.png" width="80%">

各シーンは、背景のデータを持ったシーンと、UIのみを持ったデータを持ったシーンを合成することで作成しています。

<img src="Portfolio/png/6_timeline2.png" width="80%">

遷移アニメーション時に背景のデータを持ったシーンはTimeline、<br>UIのみを持ったデータを持ったシーンはAnimationを再生することで実現しています。

また、SceneManager.LoadSceneAsyncを活用してシーンを合成するため、<br>アニメーションイベントを使用してシーンの合成、削除を動的に行っています。

#### 6.3 カットイン
<img src="Portfolio/gif/6_CutIn.gif" width="60%"><br>
「エネミーの弱点を突いた」際の効果として、カットインを実装しています。

<img src="Portfolio/png/6_Cutin.png" width="90%">

▲ 演出を開始する関数<br>
※黄色のラインの部分でカットインのアニメーションを再生。

カットインのアニメーションを用意し、
弱点を突いているならスクリプト内でカメラ制御、<br>エフェクトの生成と共にアニメーションを実行しています。

CutInStart()関数内ではアニメーションの再生に加えて<br>カットイン時に表示するキャラクターの画像、背景の色などの設定を行っています。
</dd>

### 7.使用素材
---
<dd>

[RPGドット【Rド】](http://rpgdot3319.g1.xrea.com/muz/002.html "http://rpgdot3319.g1.xrea.com/muz/002.html")<br>
七三ゆきのアトリエ<br>
[自家製フォント工房](http://jikasei.me/font/jf-dotfont/ "http://jikasei.me/font/jf-dotfont/")<br>
[ひみつ ゲーム用フリー素材](http://uros.web.fc2.com/frame.html "http://uros.web.fc2.com/frame.html")<br>
© Unity Technologies Japan/UCL<br>

</dd>

### 8.楽曲に関しまして
---
<dd>

こちらの作品に使用しております楽曲、[A Summer's Dream (向香織 様制作)](https://audiostock.jp/audio/1120600 "https://audiostock.jp/audio/1120600")
に関しまして、<br>
Audiostockにてダウンロードしたものになりますのでご了承ください。<br>

</dd>