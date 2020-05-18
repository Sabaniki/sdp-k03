using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Sequence = System.Collections.IEnumerator;

/// <summary>
/// ゲームクラス。
/// 学生が編集すべきソースコードです。
/// </summary>
public sealed class Game : GameBase {
    // 変数の宣言
    int sec = 0;
    public Vector2 ScreenSize;
    private Ball ball;
    private Player player;
    private bool isBallAlive;
    

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void InitGame() {
        ScreenSize = new Vector2(640, 480);
        ball = new Ball(new Vector2(0, 0), new Vector2(3, 3), ScreenSize);
        player = new Player(new Vector2(270, 460), new Vector2(100, 20));
        // キャンバスの大きさを設定します
        gc.SetResolution((int)ScreenSize.x, (int)ScreenSize.y);
        ball.updateCallback += () => { Debug.Log(ball.Position.ToString());};
        isBallAlive = true;
    }


    /// <summary>
    /// 動きなどの更新処理
    /// </summary>
    public override void UpdateGame() {
        // 起動からの経過時間を取得します
        sec = (int) gc.TimeSinceStartup;
        isBallAlive = ball.Update();
        player.Update(gc.GetPointerFrameCount(0), gc.GetPointerX(0));
        
    }

    /// <summary>
    /// 描画の処理
    /// </summary>
    public override void DrawGame() {
        // 画面を白で塗りつぶします
        gc.ClearScreen();

        // 0番の画像を描画します
        if (isBallAlive) {
            gc.DrawImage(0, 0, 0);
            gc.DrawImage(1, (int)ball.Position.x, (int)ball.Position.y);
        }
        
        gc.SetColor(0, 0, 255);
        gc.FillRect((int)player.Position.x, (int)player.Position.y, (int)player.Size.x, (int)player.Position.y);
    }
}

interface IGameObject {
}

class Ball: IGameObject {
    public Vector2 Position;

    public Vector2 Speed;
    
    private readonly Vector2 resolution;

    private readonly int ballRadius;

    public delegate void UpdateBallCallback();

    public UpdateBallCallback updateCallback;

    public Ball(Vector2 position, Vector2 speed, Vector2 resolution) {
        this.Position = position;
        this.Speed = speed;
        this.resolution = resolution;
        ballRadius = 24;
    }

    public bool Update() {
        Position.x += Speed.x;
        Position.y += Speed.y;
        if (Position.x < 0) {
            Position.x = 0;
            Speed.x = -Speed.x;
        }
        if (Position.y < 0) {
            Position.y = 0;
            Speed.y = -Speed.y;
        }
        if (Position.x > resolution.x - ballRadius) {
            return false;
            // Position.x = resolution.x- ballRadius;
            // Speed.x = -Speed.x;
        }
        if (Position.y > resolution.y - ballRadius) {
            Position.y = resolution.y- ballRadius;
            Speed.y = -Speed.y;
        }

        updateCallback();
        return true;
    }
}

class Player: IGameObject {
    public Vector2 Position;
    public Vector2 Size;
    public Player(Vector2 position, Vector2 size) {
        Position = position;
        Size = size;
    }

    public void Update(int pointerFrameCount, int touchedPointerX) {
        Position.x = touchedPointerX - Size.x / 2;
    }
}














