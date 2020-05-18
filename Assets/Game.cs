using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GameCanvas;
using UnityEngine;
using Sequence = System.Collections.IEnumerator;

/// <summary>
/// ゲームクラス。
/// 学生が編集すべきソースコードです。
/// </summary>
public sealed class Game : GameBase {
    // 変数の宣言
    int sec = 0;
    public Vector2 screenSize;
    private Ball ball;
    private Player player;
    private List<Block> blocks;
    [SerializeField] private int sumOfBlocks = 50;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void InitGame() {
        screenSize = new Vector2(640, 480);
        ball = new Ball(new Vector2(0, 0), new Vector2(3, 3), screenSize);
        player = new Player(new Vector2(270, 460), new Vector2(100, 20));
        // キャンバスの大きさを設定します
        gc.SetResolution((int) screenSize.x, (int) screenSize.y);
        blocks = new List<Block>();
        for (var i = 0; i < 50; i++) {
            blocks.Add(new Block(new Vector2(((i % 10) * Block.Size.x), ((i / 10) * Block.Size.y))));
        }

        ball.UpdateCallback += () => {
            if (gc.CheckHitRect(
                (int) ball.Position.x, (int) ball.Position.y, ball.BallRadius, ball.BallRadius,
                (int) player.Position.x, (int) player.Position.y, (int) player.Size.x, (int) player.Size.y
            )) {
                ball.Speed.y = -ball.Speed.y;
            }
        };

        ball.UpdateCallback += () => {
            blocks.ForEach(block => {
                if (gc.CheckHitRect(
                    (int) ball.Position.x, (int) ball.Position.y, ball.BallRadius, ball.BallRadius,
                    (int) block.Position.x, (int) block.Position.y, (int) Block.Size.x, (int) Block.Size.y
                ) && block.IsAlive) {
                    ball.Speed.y = -ball.Speed.y;
                    block.IsAlive = false;
                }
            });
        };
        
    }


    /// <summary>
    /// 動きなどの更新処理
    /// </summary>
    public override void UpdateGame() {
        // 起動からの経過時間を取得します
        sec = (int) gc.TimeSinceStartup;
        ball.Update();
        player.Update(gc.GetPointerFrameCount(0), gc.GetPointerX(0));
    }

    /// <summary>
    /// 描画の処理
    /// </summary>
    public override void DrawGame() {
        // 画面を白で塗りつぶします
        gc.ClearScreen();
        gc.DrawImage(0, 0, 0);
        // 0番の画像を描画します
        if (ball.IsAlive) gc.DrawImage(1, (int) ball.Position.x, (int) ball.Position.y);
        else gc.DrawString("Game Over!!", (int)screenSize.x / 2, (int)screenSize.y / 2);

        gc.SetColor(0, 0, 255);
        gc.FillRect((int) player.Position.x, (int) player.Position.y, (int) player.Size.x, (int) player.Position.y);

        foreach (var (block, index) in blocks.Select((block, index) => (block, index))) {
            if (!block.IsAlive) continue;
            gc.SetColor(index * (255 / blocks.Count), index * (255 / blocks.Count), index * (255 / blocks.Count));
            gc.FillRect((int) block.Position.x, (int) block.Position.y, (int) Block.Size.x, (int) Block.Size.y);
        }

        if (blocks.All(block => !block.IsAlive)) {
            gc.DrawString("Game Clear!!", (int)screenSize.x / 2, (int)screenSize.y / 2);
        }

    }
}

internal interface IGameObject {
}

internal class Ball : IGameObject {
    public Vector2 Position;

    public Vector2 Speed;

    private readonly Vector2 resolution;

    public readonly int BallRadius;

    public delegate void UpdateBallCallback();

    public UpdateBallCallback UpdateCallback;

    private readonly Proxy proxy;
    
    public bool IsAlive { get; private set; }


    public Ball(Vector2 position, Vector2 speed, Vector2 resolution) {
        this.Position = position;
        this.Speed = speed;
        this.resolution = resolution;
        BallRadius = 24;
        IsAlive = true;
    }

    public void Update() {
        UpdateCallback();
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

        if (Position.x > resolution.x - BallRadius) {
            Position.x = resolution.x - BallRadius;
            Speed.x = -Speed.x;
        }

        if (!(Position.y > resolution.y - BallRadius)) return;
        IsAlive = false;
    }
}

internal class Player : IGameObject {
    public Vector2 Position;
    public Vector2 Size;

    public Player(Vector2 position, Vector2 size) {
        Position = position;
        Size = size;
    }

    public void Update(int pointerFrameCount, int touchedPointerX) {
        if (pointerFrameCount > 0)
            Position.x = touchedPointerX - Size.x / 2;
    }
}

internal class Block {
    public Vector2 Position;
    public static Vector2 Size = new Vector2(64, 20);
    public bool IsAlive { get; set; }

    public Block(Vector2 position) {
        Position = position;
        IsAlive = true;
    }
}