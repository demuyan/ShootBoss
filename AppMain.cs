/*
 * Copyright (C) 2012 Narikazu Demura
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

using Sce.PlayStation.HighLevel.UI;

namespace ShootBoss
{
    // ゲーム登場キャラクター　基底クラス
	public class GameEntity : Node
	{
		public SpriteTile Sprite { get; set; }
		
		public int FrameCount { get; set; }

		public List<HitTest.CollisionEntry> CollisionDatas;
		
		public static Vector2 GetCollisionCenter (Node node)
		{
			Bounds2 bounds = new Bounds2 ();
			node.GetlContentLocalBounds (ref bounds);
			Vector2 center = node.LocalToWorld (bounds.Center);
			return center;
		}
		
		public GameEntity ()
		{
			Scheduler.Instance.Schedule (this, Tick, 0.0f, false);
			CollisionDatas = new List<HitTest.CollisionEntry> ();
		}

		public virtual SpriteTile createSprite(TextureInfo texture_info, float world_scale=1.5f){
			
			SpriteTile sprite;

			// スプライト生成
			sprite = new SpriteTile () { TextureInfo = texture_info};
			// 表示するテクスチャーID
			sprite.TileIndex1D = 0;
			// 拡大率
			sprite.Quad.S = sprite.CalcSizeInPixels () * world_scale; 
			// スプライトの中心と座標の中心を一致させる
			sprite.CenterSprite ();
			// 輝度設定
			sprite.Color = new Vector4 (1.0f, 1.0f, 1.0f, 1.00f);
			// 表示の混合設定
			sprite.BlendMode = Sce.PlayStation.HighLevel.GameEngine2D.Base.BlendMode.Normal;
			
			return sprite;
		}
		

		public virtual void Tick (float dt)
		{
			if (Game.Instance.Pause)
				return;

			FrameCount += 1;
			
			foreach (HitTest.CollisionEntry c in CollisionDatas) {
				if (c.owner != null)
					Game.Instance.HitTest.Add (c);
			}
		}
		
		public virtual void Hit (GameEntity owner)
		{
		}
	}
	
	// 自機（プレイヤー）
	public class Player : GameEntity
	{
		public int Damage = 0;
		Boolean Shoot { get; set; }
		
		public Player (Vector2 pos)
		{
			// 座標設定
			this.Position = pos;

			// スプライト生成
			var texture_info = new TextureInfo (new Texture2D ("/Application/assets/myship.png", false), new Vector2i (1, 1));
			Sprite = createSprite(texture_info);
            
			// スプライトをNodeに追加する
			this.AddChild (Sprite);
			
            // 当たり判定データ登録
			CollisionDatas.Add (new HitTest.CollisionEntry () {
	            type = HitTest.CollisionEntityType.Player,
				owner = this,
				center = () => GetCollisionCenter (Sprite),
				radius = () => 14.0f,
			});
		}

        // 周期処理
		public override void Tick (float dt)
		{
			const float speed = 300;

			Vector2 position = this.Position;
			
			base.Tick (dt);
			if (Game.Instance.Pause)
				return;
			
			// 上キーが押されている
			if (Input2.GamePad0.Up.Down) {
				position.Y += speed * dt;
				// 画面外へ移動しないように
				if (position.Y > Game.Instance.ScreenSize.Y) {
					position.Y = Game.Instance.ScreenSize.Y;
				}
			}
			
			// 下キーが押されている
			if (Input2.GamePad0.Down.Down) {
				position.Y -= speed * dt;
				// 画面外へ移動しないように
				if (position.Y < 0) {
					position.Y = 0;
				}
			}
			
			// 左キーが押されている
			if (Input2.GamePad0.Left.Down) {
				position.X -= speed * dt;
				// 画面外へ移動しないように
				if (position.X < 0) {
					position.X = 0;
				}
			}

			// 右キーが押されている
			if (Input2.GamePad0.Right.Down) {
				position.X += speed * dt;
				// 画面外へ移動しないように
				if (position.X > Game.Instance.ScreenSize.X) {
					position.X = Game.Instance.ScreenSize.X;
				}
			}
			
			Shoot = false;
			if (Input2.GamePad0.Circle.Press)
				Shoot = true;
			
            // 自弾を飛ばす
			if (Shoot) {
				Game.Instance.AddQueue.Add (new PlayerBullet (position));
				Game.Instance.Sound.Play ();
			}
			
			// 表示位置に反映する
			this.Position = position;
		}
		
		public override void Hit (GameEntity owner)
		{
			base.Hit (owner);
			Damage += 1;
		}
	}

	// ボスキャラ
	public class Boss : GameEntity
	{
		int direction = 1;
		float counter = 0;
		public int Damage = 0;
		
		public Boss (Vector2 pos)
		{
			// 座標設定
			this.Position = pos;
			// スプライト生成
			var texture_info = new TextureInfo (new Texture2D ("/Application/assets/boss.png", false), new Vector2i (1, 1));
			Sprite = createSprite(texture_info);
			// スプライトをNodeに追加する
			this.AddChild (Sprite);

            // 当たり判定データ登録
			CollisionDatas.Add (new HitTest.CollisionEntry () {
	            type = HitTest.CollisionEntityType.Boss,
				owner = this,
				center = () => GetCollisionCenter (Sprite),
				radius = () => 32.0f,
			});

		}

        // 周期処理
		public override void Tick (float dt)
		{
			const float speed = 80;
			
			base.Tick (dt);
			if (Game.Instance.Pause)
				return;

            // SIN波の動きで移動
			var pos = this.Position;
			pos.Y += 1.7f * FMath.Sin (FrameCount * 0.015f);
			pos.X += speed * dt * direction;
			this.Position = pos;
			if (direction == 1) {
				if (this.Position.X > Game.Instance.ScreenSize.X - 100) {
					direction = -1;
				}
			} else {
				if (this.Position.X < 100) {
					direction = 1;
				}
			}
			
			counter += dt;

            // 敵弾を発射する周期を算出する
			float countBase = 1;
			if (Game.Instance.OptionDialog != null)
				countBase = 1 - 0.25f * Game.Instance.OptionDialog.BulletInterval / 100;

			if (counter > countBase) {
				
                // 敵弾を発射
				float[] angles = new float[]{
                     FMath.PI * 4 / 8,  FMath.PI * 3 / 8,  FMath.PI * 2 / 8,  FMath.PI * 1 / 8,0,
					-FMath.PI * 4 / 8, -FMath.PI * 3 / 8, -FMath.PI * 2 / 8, -FMath.PI * 1 / 8};
				Vector2 srcvec = new Vector2 (0, -100f);
				foreach(float val in angles){
					Game.Instance.AddQueue.Add (new BossBullet (this.Position, srcvec.Rotate (val))); 	
				}
				counter = 0;
			}
		}

        // あたった時の処理
		public override void Hit (GameEntity owner)
		{
			base.Hit (owner);
			
//			Console.WriteLine ("Hit!!");
		//	this.RemoveChild (owner.Sprite, true);
			if (owner is PlayerBullet) {
				this.RemoveChild (owner.Sprite, true);
				Game.Instance.RemoveQueue.Add (owner);
			}
            // ダメージ加算
			Damage += 1;
		}
	}

	// 敵弾
	public class BossBullet : GameEntity
	{
		Vector2 velocity;
		
		public BossBullet (Vector2 pos, Vector2 velocity)
		{
			// 座標設定
			this.Position = pos;
            // 移動方向（加速度）設定
			this.velocity = velocity;
			// テクスチャーの呼び出し
			var texture_info = new TextureInfo (new Texture2D ("/Application/assets/bossbullet.png", false), new Vector2i (1, 1));
			Sprite = createSprite(texture_info, 1.0f);
			
			// スプライトをNodeに追加する
			this.AddChild (Sprite);

            // 当たり判定データ登録
			CollisionDatas.Add (new HitTest.CollisionEntry () {
	            type = HitTest.CollisionEntityType.BossBullet,
				owner = this,
				center = () => GetCollisionCenter (Sprite),
				radius = () => 3.0f,
			});
		}

		public override void Tick (float dt)
		{
			base.Tick (dt);
			
			if (Game.Instance.Pause)
				return;
            
            // 移動
			var pos = this.Position;
			pos.X += velocity.X * dt;
			pos.Y += velocity.Y * dt;
			this.Position = pos;
			
            // 画面外へ行ったら消す
			if ((pos.Y > Game.Instance.ScreenSize.Y) || 
			    (pos.Y < -32) || 
			    (pos.X < -32) || 
			    (pos.X > Game.Instance.ScreenSize.X)) {
				this.RemoveChild (Sprite, true);
				Game.Instance.RemoveQueue.Add (this);
			}
		}
	}
	
	// 自弾
	public class PlayerBullet : GameEntity
	{
		
		public PlayerBullet ():this(new Vector2(0,0))
		{
		}
		
		public PlayerBullet (Vector2 pos)
		{
            // 座標設定
			this.Position = pos;

			// スプライト生成
			var texture_info = new TextureInfo (new Texture2D ("/Application/assets/mybullet.png", false), new Vector2i (1, 1));
			Sprite = createSprite(texture_info);

			// スプライトをNodeに追加する
			this.AddChild (Sprite);
			
            // 当たり判定データ登録
			CollisionDatas.Add (new HitTest.CollisionEntry () {
	            type = HitTest.CollisionEntityType.PlayerBullet,
				owner = this,
				center = () => GetCollisionCenter (Sprite),
				radius = () => 16.0f,
			});
		}
		
		public override void Tick (float dt)
		{
            // スピード指定
			const float speed = 300;
			
			base.Tick (dt);
			
			if (Game.Instance.Pause)
				return;
            
			var pos = this.Position;
			pos.Y += speed*dt;
			this.Position = pos;

            // 画面外へ行ったら消す
			if (pos.Y > Game.Instance.ScreenSize.Y) {
				this.RemoveChild (Sprite, true);
				Game.Instance.RemoveQueue.Add (this);
			}
		}
	}

	// 背景
	public class BackGround : GameEntity
	{
		Vector2 position;
		TextureInfo texture_info;
		
		public BackGround ():this(new Vector2(0,0))
		{
		}

		public BackGround (Vector2 pos)
		{
			// 座標設定
			position = pos;
			
			// スプライト生成
			texture_info = new TextureInfo (new Texture2D ("/Application/assets/background.png", false), new Vector2i (1, 1));
			Sprite = createSprite(texture_info, 1.00f);

			Sprite.Position = new Vector2 (0, 0);
			this.Position = pos;
			// スプライトをNodeに追加する
			this.AddChild (Sprite);
		}

		public override void Tick (float dt)
		{
			if (Game.Instance.Pause)
				return;

			base.Tick (dt);

			var pos = this.Position;
			pos.Y -= 5;
			// 背景を画面上部に移動する
			if (pos.Y < -128)
				pos.Y += 128*6;			
			this.Position = pos;			
		}
	}

    // 当たり判定
	public class HitTest
	{
		public enum CollisionEntityType
		{
			Player,       // 自機
			PlayerBullet, // 自弾
			Boss,         // ボスキャラ
			BossBullet    // 敵弾
		}
		
		public delegate Vector2 GetCenterDelegate ();

		public delegate float GetRadiusDelegate ();
		
		public struct CollisionEntry
		{
			public CollisionEntityType type; // キャラクターの種類
			public GameEntity owner; // 当たり判定のオブジェクト
			public GetCenterDelegate center; // 中心の座標
			public GetRadiusDelegate radius; // 半径の大きさ
		}
		
		List<List<CollisionEntry>> typed_entries;
		
		public HitTest ()
		{
			typed_entries = new List<List<CollisionEntry>> ();
			typed_entries.Add (new List<CollisionEntry> ()); // Player
			typed_entries.Add (new List<CollisionEntry> ()); // PlayerBullet
			typed_entries.Add (new List<CollisionEntry> ()); // Boss
			typed_entries.Add (new List<CollisionEntry> ()); // BossBullet
		}
		
		public void Add (CollisionEntityType type, GameEntity owner, GetCenterDelegate center, GetRadiusDelegate radius)
		{	
			CollisionEntry entry = new CollisionEntry () { type = type, owner = owner, center = center, radius = radius };
			List<CollisionEntry> entries = typed_entries [(int)type];
			entries.Add (entry);
		}
		
		public void Add (CollisionEntry entry)
		{
			List<CollisionEntry> entries = typed_entries [(int)entry.type];
			entries.Add (entry);
		}

        // 円同士で当たり判定を取る
		private void CollideEntities (List<CollisionEntry> collider, List<CollisionEntry> collidee)
		{
			for (int i = 0; i < collider.Count; ++i) {
				GameEntity collider_owner = collider [i].owner;
				Vector2 collider_center = collider [i].center ();
				float collider_radius = collider [i].radius ();
						
				for (int j = 0; j < collidee.Count; ++j) {
					GameEntity collidee_owner = collidee [j].owner;
					// 同じオブジェクト同士だったら次へ
					if (collider_owner == collidee_owner)
						continue;
					// 中心座標と半径を取得		
					Vector2 collidee_center = collidee [j].center ();
					float collidee_radius = collidee [j].radius ();
					float r = collider_radius + collidee_radius;

                    // 中心座標同士の距離を取得
					Vector2 offset = collidee_center - collider_center;
					float lensqr = offset.LengthSquared ();	
							
                    // 当たり判定
					if (lensqr < r * r) {
                        // 衝突した
						collider_owner.Hit (collidee_owner);
						collidee_owner.Hit (collider_owner);
					}
				}
			}
		}

        // 当たり判定
		public void Collide ()
		{	
            // ボスキャラと自機
			CollideEntities (typed_entries [(int)CollisionEntityType.Boss], typed_entries [(int)CollisionEntityType.Player]);
            // 敵弾と自機
			CollideEntities (typed_entries [(int)CollisionEntityType.BossBullet], typed_entries [(int)CollisionEntityType.Player]);
            // 自弾とボスキャラ
			CollideEntities (typed_entries [(int)CollisionEntityType.PlayerBullet], typed_entries [(int)CollisionEntityType.Boss]);
			Clear (); // データをクリア
		}
		
		public void Clear ()
		{
			foreach (List<CollisionEntry> entries in typed_entries)
				entries.Clear ();
		}
	}

    // ユーザーインターフェイス
	public class UI : Node
	{
        // 敵が受けたダメージ表示用ラベル
		public Sce.PlayStation.HighLevel.GameEngine2D.Label BossDamage { get; set; }

        // プレイヤーが受けたダメージ表示用ラベル
		public Sce.PlayStation.HighLevel.GameEngine2D.Label PlayerDamage { get; set; }

		public Font Font { get; set; }

		public FontMap FontMap { get; set; }

		public UI ()
		{
            // アドホックに描画する関数を指定する
			this.AdHocDraw += this.Draw;

            // ラベルの作成
			BossDamage = new Sce.PlayStation.HighLevel.GameEngine2D.Label ();
			PlayerDamage = new Sce.PlayStation.HighLevel.GameEngine2D.Label ();

            // TrueTypeフォントの読み込み
			Font = new Font ("/Application/assets/IndieFlower.ttf", 48, FontStyle.Bold);
			FontMap = new FontMap (Font);

            // フォントの設定			
			BossDamage.FontMap = FontMap;
			PlayerDamage.FontMap = FontMap;

            // ラベル表示位置設定
			BossDamage.Position = new Vector2 (120.0f, Game.Instance.ScreenSize.Y - 40);
			PlayerDamage.Position = new Vector2 (120.0f, Game.Instance.ScreenSize.Y - 80);
		
            // ラベルをNodeに追加（ラベルが表示される）
			this.AddChild (BossDamage);
			this.AddChild (PlayerDamage);

		}
		
		public new void Draw ()
		{
            // 表示色設定
			BossDamage.Color = new Vector4 (1.0f, 1.0f, 0, 1.0f);
			PlayerDamage.Color = new Vector4 (0.5f, 1.0f, 1.0f, 1.0f);
			
            // 表示内容を設定
			BossDamage.Text = String.Format ("Boss:{0}", Game.Instance.Boss.Damage);
			PlayerDamage.Text = String.Format ("Player:{0}", Game.Instance.Player.Damage);
			
		}
	}
	
	// ゲーム本体
	public class Game
	{
		public static Game Instance;

        // ワールド座標
		public Node World { get; set; }
        // UI(インターフェイス)
	//	public Node Interface { get; set; }
        // 自機
		public Player Player { get; set; }
        // 敵機（ボス）
		public Boss Boss { get; set; }
		
        // 当たり判定処理
		public HitTest HitTest { get; set; }

        // ポーズ処理
		public bool Pause { get; set; }
		public int PauseTimer { get; set; }
		public List<GameEntity> AddQueue { get; set; }
		public List<GameEntity> RemoveQueue { get; set; }
		
		public Vector2i ScreenSize { get; set; }
		
		public UIOptionDialog OptionDialog {
			get;
			private set;
		}
		
		public UI UI { get; set; }
		
		public SoundPlayer Sound{ get; set; }
		
		private static Bgm bgm;
		private static BgmPlayer player;

        // 初期化
		public void Initialize ()
		{
            // GameEngine2D 画面クリアー色の設定
			Director.Instance.GL.Context.SetClearColor (Colors.Grey20);
			
            // 画面解像度情報
//			ScreenSize = new Vector2i (854, 480); 
			ScreenSize = new Vector2i (960, 544); 
			
            // 効果音の設定
			var soundObj = new Sound ("/Application/assets/se.wav");
			Sound = soundObj.CreatePlayer ();

            // BGMの設定
			bgm = new Bgm ("/Application/assets/bgm.mp3");
			player = bgm.CreatePlayer ();
			player.Loop = true; // リピート再生する
            // BGMの再生
			player.Play ();

            // シーン生成（ノードのルートになる）
			var scene = new Sce.PlayStation.HighLevel.GameEngine2D.Scene ();
            
            // ２Dゲーム向けにカメラを設定
			scene.Camera.SetViewFromViewport ();

            // GameEngine2Dを実行			
			Director.Instance.RunWithScene (scene, true);
			
			Pause = false;
			
			World = new Node ();
		//	Interface = new Node ();
            // シーンにWorldを追加する
			scene.AddChild (World);
            // シーンにInterface(UIで利用する)を追加する
		//	scene.AddChild (Interface);

            // 背景登録
			for (int y = 0; y < 7; y++) {
				for (int x = 0; x < 6; x++) {
					var backGround = new BackGround (new Vector2 (x * 192, y * 128));
					World.AddChild (backGround);
				}
			}

            // 自機（プレイヤー）登録
			Player = new Player (new Vector2 (Game.Instance.ScreenSize.X/2, 100));
			World.AddChild (Player);
            // 敵機（ボス）登録
			Boss = new Boss (new Vector2 (Game.Instance.ScreenSize.X/2, 250));
			World.AddChild (Boss);
            // UI登録
			UI = new UI ();
			//Interface.AddChild (UI);
			scene.AddChild (UI);
            // 当たり判定処理の生成
			HitTest = new HitTest ();
            // 登録キュー
			AddQueue = new List<GameEntity> ();
            // 削除キュー
			RemoveQueue = new List<GameEntity> ();
		}

        // 定期的なアップデート処理
		public void FrameUpdate ()
		{
            // ポーズ処理			
			if (Input2.GamePad0.Start.Press) {
				
				if (!this.Pause) {
					this.Pause = !this.Pause;
                    // オプションダイアログの表示
					OptionDialog = new UIOptionDialog ();
					OptionDialog.Show ();
				}
			}
			
            // ポーズ解除をタイミング
			if (PauseTimer > 0) {
				PauseTimer -= 1;
				if (PauseTimer <= 0){
					this.Pause = false;
				}
			}

            // 当たり判定処理			
			HitTest.Collide ();

            // 削除キューにあるNodeをWorldから取り除く
			foreach (GameEntity e in RemoveQueue) 
				World.RemoveChild (e, true);
			
            // 登録キューにあるNodeをWorldに追加する			
			foreach (GameEntity e in AddQueue)
				World.AddChild (e);
			
//			Console.WriteLine ("World.Children.Count={0}", World.Children.Count);

            // 削除キュー、登録キューをクリアーする			
			RemoveQueue.Clear ();	
			AddQueue.Clear ();
		}
	}
	
	public class AppMain
	{
        // メイン関数
		public static void Main (string[] args)
		{
			Initialize ();

			while (!Input2.GamePad0.Select.Press) {
				SystemEvents.CheckEvents ();
				Update ();
				Render ();
			}
		}

        // PSSuite 初期化処理
		public static void Initialize ()
		{
            // コンソールへのログ出力設定
//			Log.SetToConsole ();
			
			// グラフィックスコンテキストの生成
			GraphicsContext context = new GraphicsContext ();

			// 利用するスプライトの最大数を設定する
			uint sprites_capacity = 500;

			// デバッグ用描画で利用する頂点の最大数を設定する
			uint draw_helpers_capacity = 400;

			// GameEngine2Dの初期化
			Director.Initialize (sprites_capacity, draw_helpers_capacity, context);

            // Gameインスタンスの生成、初期化
			Game.Instance = new Game ();
			Game.Instance.Initialize ();

            // UIの初期化			
			UISystem.Initialize (context);

		}

        // PSSuite 更新処理
		public static void Update ()
		{
			// ゲームパッド入力を取得
			var gamePadData = GamePad.GetData (0);
            // タッチパネル入力を取得
			List<TouchData> touchData = Touch.GetData (0);
            
			var game = Game.Instance;
			// ゲーム処理を更新
            game.FrameUpdate ();
            // GameEngine2Dの内容を更新
			Director.Instance.Update ();
            // UIにタッチパネル入力を反映
			UISystem.Update (touchData);
		}

        // PSSuite 描画処理
		public static void Render ()
		{
            // GameEngine2Dでの描画処理
			Director.Instance.Render ();
			// UIの描画処理
			UISystem.Render ();

			// フレームバッファのスワップを行う
			Director.Instance.GL.Context.SwapBuffers ();
			Director.Instance.PostSwap (); // PostSwapはSwapBuffersの呼び出し後に処理すること！！
		}
	}
}
