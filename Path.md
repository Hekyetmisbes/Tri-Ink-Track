# Triâ€‘Ink Track â€” GeliÅŸtirme Yol HaritasÄ±

> **Proje:** Triâ€‘Ink Track (ÃœÃ§ MÃ¼rekkep Parkuru)
> **Engine:** Unity 6000.3.9f1 â€” URP 2D
> **Platform:** iOS / Android (Mobil)
> **Tarih:** 2026-02-25

---

## Mevcut Proje Durumu

| Kategori | Durum |
|---|---|
| Unity Projesi | Kurulu (URP 2D, Input System) |
| Sahne | `SampleScene.unity` â€” Kamera + Global Light 2D |
| Sprite'lar | Kenney Physics Assets, Rolling Ball Assets (~500+ sprite) |
| UI Elementleri | Kenney UI Pack (butonlar, checkbox'lar, slider'lar) |
| Fontlar | Kenney Future, Kenney Future Narrow |
| Ses Efektleri | Kenney Interface Sounds (100+ OGG) |
| ParÃ§acÄ±k Efektleri | Unity Particle Pack (Ã¶rnek) |
| Oyun Script'leri | **YOK â€” SÄ±fÄ±rdan yazÄ±lacak** |
| Fizik Materyalleri | **YOK â€” OluÅŸturulacak** |
| Prefab'lar | **YOK â€” OluÅŸturulacak** |
| Level TasarÄ±mlarÄ± | **YOK â€” Tasarlanacak** |

---

## Hedef KlasÃ¶r YapÄ±sÄ±

```
Assets/
â”œâ”€â”€ _Game/
â”‚   â”œâ”€â”€ Art/
â”‚   â”‚   â”œâ”€â”€ Sprites/          â† Ball, Target, Hazard, Ink gÃ¶rselleri
â”‚   â”‚   â””â”€â”€ Backgrounds/      â† Level arka planlarÄ±
â”‚   â”œâ”€â”€ Audio/
â”‚   â”‚   â”œâ”€â”€ SFX/              â† Oyun sesleri (Ã§izim, Ã§arpma, win, fail)
â”‚   â”‚   â””â”€â”€ Music/            â† (MVP sonrasÄ±)
â”‚   â”œâ”€â”€ Fonts/                â† Kenney Future fontlarÄ± (referans)
â”‚   â”œâ”€â”€ PhysicsMaterials/     â† Ice, Sticky, Bouncy
â”‚   â”œâ”€â”€ Prefabs/
â”‚   â”‚   â”œâ”€â”€ Ball.prefab
â”‚   â”‚   â”œâ”€â”€ Target.prefab
â”‚   â”‚   â”œâ”€â”€ InkLine.prefab
â”‚   â”‚   â”œâ”€â”€ Hazard.prefab
â”‚   â”‚   â”œâ”€â”€ Wall.prefab
â”‚   â”‚   â””â”€â”€ Levels/
â”‚   â”‚       â”œâ”€â”€ Level_01.prefab â€¦ Level_10.prefab
â”‚   â”œâ”€â”€ Scenes/
â”‚   â”‚   â””â”€â”€ GameScene.unity
â”‚   â”œâ”€â”€ Scripts/
â”‚   â”‚   â”œâ”€â”€ Core/
â”‚   â”‚   â”‚   â”œâ”€â”€ GameManager.cs
â”‚   â”‚   â”‚   â””â”€â”€ LevelManager.cs
â”‚   â”‚   â”œâ”€â”€ Ball/
â”‚   â”‚   â”‚   â””â”€â”€ BallController.cs
â”‚   â”‚   â”œâ”€â”€ Drawing/
â”‚   â”‚   â”‚   â”œâ”€â”€ DrawSystem.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ InkLine.cs
â”‚   â”‚   â”‚   â”œâ”€â”€ InkLineLifetime.cs
â”‚   â”‚   â”‚   â””â”€â”€ InkLinePool.cs
â”‚   â”‚   â”œâ”€â”€ Ink/
â”‚   â”‚   â”‚   â”œâ”€â”€ InkType.cs           (enum)
â”‚   â”‚   â”‚   â””â”€â”€ InkInventory.cs
â”‚   â”‚   â”œâ”€â”€ UI/
â”‚   â”‚   â”‚   â””â”€â”€ UIHudController.cs
â”‚   â”‚   â”œâ”€â”€ Audio/
â”‚   â”‚   â”‚   â””â”€â”€ AudioManager.cs
â”‚   â”‚   â”œâ”€â”€ Vfx/
â”‚   â”‚   â”‚   â””â”€â”€ VfxManager.cs       (opsiyonel MVP)
â”‚   â”‚   â””â”€â”€ Config/
â”‚   â”‚       â”œâ”€â”€ GameConfig.cs         (ScriptableObject)
â”‚   â”‚       â””â”€â”€ LevelDefinition.cs    (ScriptableObject)
â”‚   â”œâ”€â”€ ScriptableObjects/
â”‚   â”‚   â”œâ”€â”€ GameConfig.asset
â”‚   â”‚   â””â”€â”€ Levels/
â”‚   â”‚       â”œâ”€â”€ Level_01_Def.asset â€¦ Level_10_Def.asset
â”‚   â””â”€â”€ UI/
â”‚       â”œâ”€â”€ HUD.prefab
â”‚       â””â”€â”€ Sprites/          â† UI ikonlarÄ± (ink seÃ§im, retry, vb.)
â”œâ”€â”€ kenney_physics-assets/     â† (Mevcut)
â”œâ”€â”€ kenney_rolling-ball-assets/ â† (Mevcut)
â”œâ”€â”€ kenney_ui-pack/            â† (Mevcut)
â”œâ”€â”€ kenney_interface-sounds/   â† (Mevcut)
â””â”€â”€ Settings/                  â† (Mevcut â€” URP ayarlarÄ±)
```

---

## KullanÄ±lacak Mevcut Asset EÅŸleÅŸtirmesi

| Oyun Elementi | Kaynak Asset | Dosya Yolu |
|---|---|---|
| Bilye (Ball) | `ball_blue_large.png` | `kenney_rolling-ball-assets/PNG/Default/ball_blue_large.png` |
| Hedef (Target) | `hole_end.png` | `kenney_rolling-ball-assets/PNG/Default/hole_end.png` |
| BaÅŸlangÄ±Ã§ | `hole_start.png` | `kenney_rolling-ball-assets/PNG/Default/hole_start.png` |
| Duvar bloklarÄ± | `block_large.png`, `block_narrow.png` | `kenney_rolling-ball-assets/PNG/Default/block_*.png` |
| Sabit engeller | `locked_*` | `kenney_rolling-ball-assets/PNG/Default/locked_*.png` |
| Arka plan | `background_blue.png` vb. | `kenney_rolling-ball-assets/PNG/Default/background_*.png` |
| YÄ±ldÄ±z (skor) | `star.png`, `star_outline.png` | `kenney_rolling-ball-assets/PNG/Default/star*.png` |
| UI butonlarÄ± | `button_rectangle_flat.png` | `kenney_ui-pack/PNG/*/button_rectangle_*.png` |
| Ink seÃ§im ikonlarÄ± | `icon_circle.png` (renklendirilmiÅŸ) | `kenney_ui-pack/PNG/*/icon_circle*.png` |
| Checkbox | `checkbox_*.png` | `kenney_ui-pack/PNG/*/checkbox_*.png` |
| Retry/Next buton | `button_round_depth_flat.png` | `kenney_ui-pack/PNG/*/button_round_*.png` |
| Font | Kenney Future | `kenney_ui-pack/Fonts/Kenney Future.ttf` |
| TÄ±klama sesi | `click_003.ogg` | `kenney_interface-sounds/Audio/click_003.ogg` |
| Ã‡izim sesi | `scratch_003.ogg` | `kenney_interface-sounds/Audio/scratch_003.ogg` |
| Win sesi | `confirmation_002.ogg` | `kenney_interface-sounds/Audio/confirmation_002.ogg` |
| Fail sesi | `error_004.ogg` | `kenney_interface-sounds/Audio/error_004.ogg` |
| Buton hover | `select_002.ogg` | `kenney_interface-sounds/Audio/select_002.ogg` |
| Ink seÃ§im sesi | `switch_003.ogg` | `kenney_interface-sounds/Audio/switch_003.ogg` |
| Bouncy sekme | `glass_002.ogg` | `kenney_interface-sounds/Audio/glass_002.ogg` |

---

## Script Listesi ve SorumluluklarÄ±

| Script | Sorumluluk |
|---|---|
| `GameManager.cs` | Game state yÃ¶netimi (Ready â†’ Playing â†’ Win/Fail â†’ Retry/Next) |
| `LevelManager.cs` | Level yÃ¼kleme, sÄ±fÄ±rlama, ilerleme kaydetme |
| `BallController.cs` | Rigidbody2D auto-move, hÄ±z normalizasyonu, reset |
| `DrawSystem.cs` | Touch/mouse input â†’ point sampling â†’ InkLine spawn |
| `InkLine.cs` | LineRenderer + EdgeCollider2D senkronizasyonu |
| `InkLineLifetime.cs` | Timer, fade animasyonu, collider disable, pool'a iade |
| `InkLinePool.cs` | Object pooling (ink tipi baÅŸÄ±na) |
| `InkType.cs` | `enum InkType { Ice, Sticky, Bouncy }` |
| `InkInventory.cs` | Ink puanÄ± tÃ¼ketimi ve kalan miktar takibi |
| `UIHudController.cs` | Ink seÃ§im butonlarÄ±, ink bar, retry/next UI |
| `AudioManager.cs` | SFX tetikleme (singleton) |
| `GameConfig.cs` | ScriptableObject â€” global oyun parametreleri |
| `LevelDefinition.cs` | ScriptableObject â€” level baÅŸÄ±na konfigÃ¼rasyon |
| `VfxManager.cs` | Basit VFX tetikleme â€” win/fail parÃ§acÄ±k efektleri (opsiyonel MVP) |

---

## Kod StandartlarÄ± (Referans)

> AGENT.md BÃ¶lÃ¼m 11'den â€” tÃ¼m adÄ±mlarda uygulanacak kurallar.

- Public API minimize: `[SerializeField] private` tercih et
- `Update`/`FixedUpdate` iÃ§inde allocation yapma (no `new`, no `string +`)
- Pooling: `Instantiate`/`Destroy` dÃ¶ngÃ¼sÃ¼nden kaÃ§Ä±n
- Ã‡izgi noktalarÄ± iÃ§in `List<Vector3>` yeniden kullan (`Clear()` + reuse)
- Parametreleri `ScriptableObject` veya `[Serializable]` config struct ile tek yerde topla
- Debug loglarÄ± `#if UNITY_EDITOR` ile kontrol et

---

# ADIMLAR

---

## Step 1 â€” Bootstrap: Proje AltyapÄ±sÄ± ve GameScene
**Branch:** `feature/step1-bootstrap`

### KlasÃ¶r ve Sahne HazÄ±rlÄ±ÄŸÄ±
- [x] `Assets/_Game/` ana klasÃ¶r yapÄ±sÄ±nÄ± oluÅŸtur (Art, Audio, Scripts, Prefabs, Scenes, PhysicsMaterials, ScriptableObjects, UI)
- [x] `Assets/_Game/Scenes/GameScene.unity` oluÅŸtur (SampleScene iÃ§eriÄŸi temel alÄ±ndÄ±; SampleScene yerinde kaldÄ±)
- [x] EditorBuildSettings'e GameScene'i ekle
- [x] Kamera ayarlarÄ±nÄ± doÄŸrula: Orthographic, 2D, size = 10
- [x] Global Light 2D ayarlarÄ±nÄ± doÄŸrula

### GameManager (State Machine)
- [x] `Assets/_Game/Scripts/Core/GameManager.cs` oluÅŸtur
- [x] `GameState` enum tanÄ±mla: `Ready, Playing, Win, Fail`
- [x] Singleton pattern uygula (DontDestroyOnLoad kullanma â€” tek sahne MVP)
- [x] State geÃ§iÅŸ metodlarÄ±: `StartGame()`, `OnWin()`, `OnFail()`, `Retry()`, `NextLevel()`
- [x] `OnGameStateChanged` event tanÄ±mla (diÄŸer sistemlerin dinlemesi iÃ§in)

### GameConfig (ScriptableObject)
- [x] `Assets/_Game/Scripts/Config/GameConfig.cs` oluÅŸtur
- [x] Temel parametreler: `ballSpeed`, `inkLifetime`, `maxActiveLines`, `maxPointsPerLine`, `minPointDist`, `totalInkPoints`
- [x] `Assets/_Game/ScriptableObjects/GameConfig.asset` oluÅŸtur

### Sahne HiyerarÅŸisi
- [x] GameScene'de boÅŸ GameObject'ler oluÅŸtur: `GameManager`, `LevelRoot`, `DrawSystem`, `UI Canvas`
- [x] GameManager script'ini GameManager objesine ata
- [x] GameConfig asset'ini GameManager'a referans olarak baÄŸla

### DoÄŸrulama
- [ ] Play modunda GameManager singleton eriÅŸilebilir
- [ ] State geÃ§iÅŸleri Console'da loglanÄ±yor
- [ ] Proje hatasÄ±z derleniyor

---

## Step 2 â€” Ball + Target: Temel Hareket ve Win KoÅŸulu
**Branch:** `feature/step2-ball-target`

### Ball Prefab
- [x] `Assets/_Game/Prefabs/Ball.prefab` oluÅŸtur
- [x] Sprite: `kenney_rolling-ball-assets/PNG/Default/ball_blue_large.png`
- [x] BileÅŸenler: `SpriteRenderer`, `Rigidbody2D`, `CircleCollider2D`
- [x] Rigidbody2D ayarlarÄ±: `gravityScale = 0`, `linearDrag = 0.1â€“0.4` (tweak), `angularDrag = 0.5`, `collisionDetection = Continuous`
- [x] Tag: `Ball`

### BallController Script
- [x] `Assets/_Game/Scripts/Ball/BallController.cs` oluÅŸtur
- [x] `[SerializeField] private float targetSpeed = 5f` (4â€“7 arasÄ± tweak)
- [x] `[SerializeField] private Vector2 initialDirection = Vector2.right`
- [x] `FixedUpdate` iÃ§inde hÄ±z normalizasyonu: `rb.velocity = rb.velocity.normalized * targetSpeed`
- [x] Ã‡ok dÃ¼ÅŸÃ¼k hÄ±z fallback'i: velocity.magnitude < 0.1f ise `initialDirection * targetSpeed` uygula
- [x] Bouncy Ã§arpÄ±ÅŸma sonrasÄ± `maxSpeed` clamp (aÅŸÄ±rÄ± hÄ±z Ã¶nleme)
- [x] `ResetBall()` metodu: pozisyon ve hÄ±z sÄ±fÄ±rlama (level restart iÃ§in)
- [x] BaÅŸlangÄ±Ã§ pozisyonu kaydetme (`spawnPosition`)
- [x] Level prefab'Ä±ndan spawn pozisyonu ve baÅŸlangÄ±Ã§ yÃ¶nÃ¼ okuma

### Target Prefab
- [x] `Assets/_Game/Prefabs/Target.prefab` oluÅŸtur
- [x] Sprite: `kenney_rolling-ball-assets/PNG/Default/hole_small_end.png`
- [x] BileÅŸenler: `SpriteRenderer`, `CircleCollider2D (isTrigger = true)`
- [x] Basit pulse animasyonu (scale ping-pong, script veya Animation)

### Win Trigger
- [x] Target prefab'a `OnTriggerEnter2D` ile Ball temasÄ± algÄ±lama ekle (Tag: "Ball")
- [x] Temas olduÄŸunda `GameManager.Instance.OnWin()` Ã§aÄŸÄ±r
- [x] Ball'a "Ball" tag'i ata

### Wall Prefab
- [x] `Assets/_Game/Prefabs/Wall.prefab` oluÅŸtur
- [x] Sprite: `kenney_rolling-ball-assets/PNG/Default/block_large.png` (tiling)
- [x] BileÅŸen: `BoxCollider2D`
- [x] FarklÄ± boyutlar iÃ§in Ã¶lÃ§eklenebilir yapÄ±

### Test Level DÃ¼zeni
- [x] GameScene'de basit dikdÃ¶rtgen alan: 4 duvar + Ball + Target
- [x] Ball sol tarafta, Target saÄŸ tarafta
- [x] Play modunda Ball otomatik hareket ediyor ve duvarlara Ã§arpÄ±yor

### DoÄŸrulama
- [x] Ball otomatik hareket ediyor, hÄ±zÄ± sabit kalÄ±yor
- [x] Duvarlara Ã§arpÄ±nca sekmesi doÄŸal
- [x] Target'a ulaÅŸÄ±nca Win state tetikleniyor
- [x] Console'da Win log'u gÃ¶rÃ¼nÃ¼yor

---

## Step 3 â€” DrawSystem v1: Ã‡izim MekaniÄŸi
**Branch:** `feature/step3-draw-system`

### InkType Enum
- [x] `Assets/_Game/Scripts/Ink/InkType.cs` oluÅŸtur
- [x] `public enum InkType { Ice, Sticky, Bouncy }`

### DrawSystem Script
- [x] `Assets/_Game/Scripts/Drawing/DrawSystem.cs` oluÅŸtur
- [x] Mouse/Touch input algÄ±lama (Input System veya legacy â€” MVP iÃ§in legacy tercih)
- [x] `Camera.main.ScreenToWorldPoint()` ile screen â†’ world dÃ¶nÃ¼ÅŸÃ¼mÃ¼
- [x] Nokta Ã¶rnekleme: yeni nokta ancak `minPointDist` (0.15) uzaklÄ±kta ise ekle
- [x] `isDrawing` flag ile pointer down/move/up yÃ¶netimi
- [x] UI Ã¼stÃ¼ne Ã§izim engelleme: `EventSystem.current.IsPointerOverGameObject()` kontrolÃ¼
- [x] Mobil touch iÃ§in: `EventSystem.current.IsPointerOverGameObject(touch.fingerId)`

### InkLine Script
- [x] `Assets/_Game/Scripts/Drawing/InkLine.cs` oluÅŸtur
- [x] `LineRenderer` referansÄ± ve ayarlarÄ± (width, color, material)
- [x] `EdgeCollider2D` referansÄ±
- [x] `AddPoint(Vector3 worldPos)` metodu
- [x] LineRenderer gÃ¼ncelleme: `SetPositionCount()`, `SetPosition()`
- [x] EdgeCollider2D sync: world â†’ local dÃ¶nÃ¼ÅŸÃ¼mÃ¼ ile `points` dizisi gÃ¼ncelle
- [x] `maxPoints` limiti (60)

### InkLine Prefab
- [x] `Assets/_Game/Prefabs/InkLine.prefab` oluÅŸtur
- [x] BileÅŸenler: `LineRenderer`, `EdgeCollider2D`, `InkLine` script
- [x] LineRenderer ayarlarÄ±: width 0.08â€“0.12, Sprites-Default material
- [x] EdgeCollider2D: baÅŸlangÄ±Ã§ta boÅŸ

### Ã‡izim AkÄ±ÅŸÄ±
- [x] Pointer Down â†’ yeni InkLine instantiate (veya pool'dan al)
- [x] Pointer Move â†’ `InkLine.AddPoint()`
- [x] Pointer Up â†’ Ã§izgiyi "kilitle" (Ã§izim durur, Ã¶mÃ¼r geri sayÄ±mÄ± baÅŸlar)
- [x] Minimum 2 nokta yoksa Ã§izgiyi sil/iade et

### DoÄŸrulama
- [x] Mouse ile sÃ¼rÃ¼kleyince pÃ¼rÃ¼zsÃ¼z Ã§izgi oluÅŸuyor
- [x] Ã‡izgi kalÄ±nlÄ±ÄŸÄ± tutarlÄ±
- [x] Ball Ã§izgiye Ã§arpÄ±yor (EdgeCollider2D Ã§alÄ±ÅŸÄ±yor)
- [x] UI butonlarÄ±na tÄ±klayÄ±nca Ã§izim baÅŸlamÄ±yor

---

## Step 4 â€” EdgeCollider2D DoÄŸruluÄŸu ve Ã‡izgi Ä°yileÅŸtirmesi
**Branch:** `feature/step4-collider-polish`

### Collider Senkronizasyonu
- [x] `InkLine.cs` â€” EdgeCollider2D gÃ¼ncelleme optimizasyonu
- [x] World â†’ Local dÃ¶nÃ¼ÅŸÃ¼mÃ¼ doÄŸrulamasÄ±: `transform.InverseTransformPoint()`
- [x] Ã‡izgi objesinin pozisyonu (0,0,0) olmalÄ± veya offset hesaplanmalÄ±
- [x] Collider noktalarÄ±nÄ±n LineRenderer ile birebir eÅŸleÅŸtiÄŸini doÄŸrula

### Segment Limiti
- [x] `maxPoints` aÅŸÄ±lÄ±nca Ã§izimi otomatik bitir (pointer up simÃ¼le)
- [x] Toplam aktif Ã§izgi sayÄ±sÄ± kontrolÃ¼: `maxActiveLines` (30)
- [x] Limit aÅŸÄ±lÄ±nca en eski Ã§izgiyi kaldÄ±r veya yeni Ã§izim engelle

### Ã‡izim Kalitesi
- [x] `minPointDist` parametresini GameConfig'den oku
- [x] Ã‡ok kÄ±sa Ã§izgileri (< 3 nokta) otomatik sil
- [x] LineRenderer corner vertices ve end cap ayarlarÄ± (Round)

### DoÄŸrulama
- [x] Collider tam olarak Ã§izginin Ã¼stÃ¼nde
- [x] Ball Ã§izginin her noktasÄ±nda Ã§arpÄ±ÅŸÄ±yor
- [x] Segment patlamasÄ± yok (Ã§ok fazla nokta oluÅŸmuyor)
- [x] 30+ Ã§izgi Ã§izilince sistem stabil

---

## Step 5 â€” Ink Tipleri ve Fizik Materyalleri
**Branch:** `feature/step5-ink-types`

### PhysicsMaterial2D OluÅŸturma
- [x] `Assets/_Game/PhysicsMaterials/Ice.physicsMaterial2D` â€” friction: 0.02, bounciness: 0.05
- [x] `Assets/_Game/PhysicsMaterials/Sticky.physicsMaterial2D` â€” friction: 0.9, bounciness: 0
- [x] `Assets/_Game/PhysicsMaterials/Bouncy.physicsMaterial2D` â€” friction: 0.1, bounciness: 0.9

### InkLine'a Tip Atama
- [x] `InkLine.cs` â€” `InkType currentType` alanÄ± ekle
- [x] `SetInkType(InkType type)` metodu: EdgeCollider2D'ye uygun PhysicsMaterial2D ata
- [x] Her ink tipi iÃ§in farklÄ± renk: Ice â†’ Mavi (#00BFFF), Sticky â†’ Turuncu (#FF8C00), Bouncy â†’ YeÅŸil (#32CD32)
- [x] LineRenderer rengini ink tipine gÃ¶re ayarla

### InkInventory Script
- [x] `Assets/_Game/Scripts/Ink/InkInventory.cs` oluÅŸtur
- [x] `[SerializeField] private int totalInkPoints = 100`
- [x] `ConsumeInk(int amount)` â€” Ã§izim sÄ±rasÄ±nda her segment iÃ§in puan dÃ¼ÅŸ
- [x] `HasInk()` â€” kontrol
- [x] `ResetInk()` â€” level restart
- [x] `OnInkChanged` event (UI gÃ¼ncelleme iÃ§in)
- [x] SeÃ§ili ink tipi takibi: `currentInkType`

### DrawSystem â€” Ink Entegrasyonu
- [x] DrawSystem'e `InkInventory` referansÄ± ekle
- [x] Ã‡izim sÄ±rasÄ±nda ink puanÄ± tÃ¼ket (her yeni nokta = 1 puan)
- [x] Ink bitince Ã§izimi durdur
- [x] Yeni InkLine spawn'larken seÃ§ili ink tipini ata

### UI â€” Ink SeÃ§im ButonlarÄ± (Temel)
- [x] UI Canvas'a 3 buton ekle (alt bar): Ice / Sticky / Bouncy
- [x] Buton sprite'larÄ±: `kenney_ui-pack` butonlarÄ± (mavi, turuncu, yeÅŸil)
- [x] SeÃ§ili buton vurgusu (scale veya border)
- [x] Buton tÄ±klama â†’ `InkInventory.currentInkType` deÄŸiÅŸtir

### DoÄŸrulama
- [x] 3 farklÄ± renkle Ã§izgi Ã§izilebiliyor
- [x] Ice Ã§izgisinde Ball kayÄ±yor (dÃ¼ÅŸÃ¼k sÃ¼rtÃ¼nme)
- [x] Sticky Ã§izgisinde Ball yavaÅŸlÄ±yor/duruyor
- [x] Bouncy Ã§izgisinde Ball belirgin ÅŸekilde sekiyor
- [x] Ink puanÄ± azalÄ±yor, bitince Ã§izim duruyor
- [x] UI'dan ink tipi deÄŸiÅŸtirilebiliyor

---

## Step 6 â€” Lifetime, Fade ve Object Pooling
**Branch:** `feature/step6-lifetime-pooling`

### InkLineLifetime Script
- [x] `Assets/_Game/Scripts/Drawing/InkLineLifetime.cs` oluÅŸtur
- [x] `lifeSeconds` (5â€“10 sn, GameConfig'den oku)
- [x] `fadeDuration` (0.5 sn)
- [x] `spawnTime` kaydÄ±
- [x] Ã–mÃ¼r dolunca fade baÅŸlat: LineRenderer alpha 1â†’0 (Coroutine veya Update)
- [x] Fade bitince EdgeCollider2D disable
- [x] Pool'a iade: `gameObject.SetActive(false)` + pool'a geri koy

### InkLinePool Script
- [x] `Assets/_Game/Scripts/Drawing/InkLinePool.cs` oluÅŸtur
- [x] `Queue<InkLine>` pool yapÄ±sÄ±
- [x] `Get()` â€” pool'dan al veya yeni oluÅŸtur
- [x] `Return(InkLine line)` â€” sÄ±fÄ±rla ve pool'a koy
- [x] BaÅŸlangÄ±Ã§ta `initialPoolSize` (10) kadar pre-spawn
- [x] InkLine sÄ±fÄ±rlama: pozisyon, nokta sayÄ±sÄ±, renk, collider temizle

### DrawSystem â€” Pool Entegrasyonu
- [x] `Instantiate` yerine `InkLinePool.Get()` kullan
- [x] Ã‡izgi Ã¶mrÃ¼ dolunca otomatik pool'a dÃ¶nÃ¼ÅŸ
- [x] Aktif Ã§izgi sayÄ±sÄ± takibi

### Performans Kontrolleri
- [x] `Update`/`FixedUpdate` iÃ§inde allocation olmamasÄ±
- [x] `List<Vector3>` yeniden kullanÄ±mÄ± (clear + reuse)
- [x] Profiler ile GC spike kontrolÃ¼

### DoÄŸrulama
- [x] Ã‡izgiler 5â€“10 sn sonra fade oluyor
- [x] Fade sÄ±rasÄ±nda gÃ¶rsel yumuÅŸak geÃ§iÅŸ
- [x] Collider fade bitince kapanÄ±yor (Ball artÄ±k geÃ§iyor)
- [x] AynÄ± Ã§izgiyi tekrar Ã§izince pool'dan alÄ±nÄ±yor (Instantiate yok)
- [x] 50+ Ã§izgi Ã§izip silme dÃ¶ngÃ¼sÃ¼nde performans stabil

---

## Step 7 â€” Fail KoÅŸullarÄ± ve SÄ±nÄ±r Sistemi
**Branch:** `feature/step7-fail-conditions`

### Hazard Prefab
- [x] `Assets/_Game/Prefabs/Hazard.prefab` oluÅŸtur
- [x] Sprite: `kenney_physics-assets/PNG/Glass Elements/` veya kÄ±rmÄ±zÄ± tint uygulanmÄ±ÅŸ blok
- [x] BileÅŸenler: `SpriteRenderer`, `BoxCollider2D (isTrigger = true)`
- [x] "Hazard" tag'i ata

### Out-of-Bounds AlgÄ±lama
- [x] Kamera sÄ±nÄ±rlarÄ±nÄ± hesapla veya sabit sÄ±nÄ±r collider'larÄ± oluÅŸtur
- [x] 4 kenar trigger (ekran dÄ±ÅŸÄ±) â€” bÃ¼yÃ¼k BoxCollider2D (isTrigger)
- [x] "Boundary" tag'i ata

### BallController â€” Fail Tetikleme
- [x] `OnTriggerEnter2D` â€” "Hazard" veya "Boundary" tag'i kontrolÃ¼
- [x] Temas â†’ `GameManager.Instance.OnFail()`

### GameManager â€” Fail State
- [x] `OnFail()` â†’ state = Fail, event tetikle
- [x] Ball hareketi durdur (veya kinematic yap)
- [x] Retry butonu aktif

### DoÄŸrulama
- [x] Ball hazard'a deÄŸince Fail state
- [x] Ball ekran dÄ±ÅŸÄ±na Ã§Ä±kÄ±nca Fail state
- [x] Fail durumunda Ball duruyor
- [x] Console'da Fail log'u

---

## Step 8 â€” UI/HUD: Tam ArayÃ¼z
**Branch:** `feature/step8-ui-hud`

### UIHudController Script
- [x] `Assets/_Game/Scripts/UI/UIHudController.cs` oluÅŸtur
- [x] GameManager state event'lerini dinle
- [x] Panelleri state'e gÃ¶re gÃ¶ster/gizle

### HUD ElemanlarÄ±
- [x] **Ink SeÃ§im Bar (Alt):** 3 buton â€” her biri ink rengiyle, seÃ§ili olan bÃ¼yÃ¼k/parlak
- [x] **Ink MiktarÄ± GÃ¶stergesi:** Slider veya fill bar â€” kalan ink puanÄ±
- [x] **Retry Butonu:** SaÄŸ Ã¼st kÃ¶ÅŸe, her zaman gÃ¶rÃ¼nÃ¼r
- [x] **Pause Butonu (Opsiyonel):** Sol Ã¼st kÃ¶ÅŸe â€” oyunu duraklat, devam et, ana menÃ¼
- [x] **Win Paneli:** "Level Complete!" + Next Level butonu + yÄ±ldÄ±z gÃ¶sterimi
- [x] **Fail Paneli:** "Failed!" + Retry butonu
- [x] **Game Complete Paneli:** Son level sonrasÄ± tebrik ekranÄ±

### UI Sprite AtamalarÄ±
- [x] Butonlar: `kenney_ui-pack/PNG/Blue/button_rectangle_flat.png` (ve renk varyantlarÄ±)
- [x] Retry ikon: `kenney_ui-pack/PNG/Grey/icon_circle.png` (R harfi overlay)
- [x] Next ikon: `kenney_ui-pack/PNG/Green/Default/arrow_basic_e.png`
- [x] Star: `kenney_rolling-ball-assets/PNG/Default/star.png`
- [x] Font: `kenney_ui-pack/Font/Kenney Future.ttf`

### UI FonksiyonelliÄŸi
- [x] Retry butonu â†’ `GameManager.Retry()` â†’ level sÄ±fÄ±rla, ink sÄ±fÄ±rla, ball sÄ±fÄ±rla
- [x] Next butonu â†’ `GameManager.NextLevel()` â†’ sonraki level yÃ¼kle
- [x] Ink butonlarÄ± â†’ `InkInventory.currentInkType` deÄŸiÅŸtir
- [x] Ink bar â†’ `InkInventory.OnInkChanged` event'ini dinle, fill gÃ¼ncelle

### State'e GÃ¶re UI
- [x] **Ready/Playing:** HUD gÃ¶rÃ¼nÃ¼r, Win/Fail panelleri gizli
- [x] **Win:** Win paneli gÃ¶ster, HUD butonlarÄ± devre dÄ±ÅŸÄ±
- [x] **Fail:** Fail paneli gÃ¶ster, HUD butonlarÄ± devre dÄ±ÅŸÄ±

### DoÄŸrulama
- [x] TÃ¼m butonlar Ã§alÄ±ÅŸÄ±yor
- [x] Ink bar doÄŸru gÃ¼ncelleniyor
- [x] Win/Fail panelleri doÄŸru zamanda gÃ¶rÃ¼nÃ¼yor
- [x] Retry level'Ä± tamamen sÄ±fÄ±rlÄ±yor
- [x] UI Ã¼stÃ¼nde Ã§izim baÅŸlamÄ±yor

---

## Step 9 â€” Level Sistemi ve 10 Level TasarÄ±mÄ±
**Branch:** `feature/step9-levels`

### LevelManager Script
- [x] `Assets/_Game/Scripts/Core/LevelManager.cs` oluÅŸtur
- [x] Level prefab listesi: `[SerializeField] private GameObject[] levelPrefabs`
- [x] `LoadLevel(int index)` â€” mevcut level'Ä± sil, yenisini `LevelRoot` altÄ±na instantiate et
- [x] `currentLevelIndex` takibi
- [x] `RestartLevel()` â€” aynÄ± level'Ä± yeniden yÃ¼kle
- [x] `NextLevel()` â€” index++, yÃ¼kle
- [x] Son level sonrasÄ±: baÅŸa dÃ¶n veya "Game Complete" gÃ¶ster

### LevelDefinition (ScriptableObject)
- [x] `Assets/_Game/Scripts/Config/LevelDefinition.cs` oluÅŸtur
- [x] Alanlar: `levelName`, `levelPrefab`, `availableInks` (Ice/Sticky/Bouncy hangilerini kullanabilir), `inkBudget`, `targetTime` (opsiyonel)
- [x] 10 adet asset oluÅŸtur: `Level_01_Def.asset` â€¦ `Level_10_Def.asset`

### Level Prefab YapÄ±sÄ±
Her level prefab ÅŸunlarÄ± iÃ§erir:
- [x] Ball spawn noktasÄ± (Transform marker)
- [x] Target pozisyonu
- [x] Duvarlar (BoxCollider2D)
- [x] Hazard'lar (isTrigger)
- [x] Arka plan sprite

### 10 Level TasarÄ±mÄ±

**Level 01 â€” DÃ¼z Yol (Tutorial: Ã‡izim Ã–ÄŸretici)**
- [x] Basit koridor, Ball solda â†’ Target saÄŸda
- [x] Engel yok, sadece Ã§izim pratiÄŸi
- [x] KullanÄ±labilir ink: **Sticky** â€” fren etkisini gÃ¶ster
- [x] Ink bÃ¼tÃ§esi: 100 (cÃ¶mert)
- [x] Ball baÅŸlangÄ±Ã§ yÃ¶nÃ¼: â†’ (saÄŸa)

**Level 02 â€” Ä°lk DÃ¶nÃ¼ÅŸ (Bouncy TanÄ±tÄ±m)**
- [x] L ÅŸeklinde koridor
- [x] Ball dÃ¼z gidiyor, kÃ¶ÅŸeyi dÃ¶nmesi gerek
- [x] KullanÄ±labilir ink: **Bouncy** â€” yÃ¶n deÄŸiÅŸtirme Ã¶ÄŸretisi
- [x] Ink bÃ¼tÃ§esi: 80
- [x] Ball baÅŸlangÄ±Ã§ yÃ¶nÃ¼: â†’ (saÄŸa)

**Level 03 â€” Kaygan Zemin (Ice TanÄ±tÄ±m)**
- [x] GeniÅŸ alan, Ball yavaÅŸ baÅŸlÄ±yor
- [x] Ice Ã§izgisiyle hÄ±zlandÄ±rma
- [x] KullanÄ±labilir ink: **Ice** â€” hÄ±zlandÄ±rma Ã¶ÄŸretisi
- [x] Ink bÃ¼tÃ§esi: 80
- [x] Target uzak kÃ¶ÅŸede

**Level 04 â€” ÃœÃ§Ã¼nÃ¼ BirleÅŸtir**
- [x] Ball â†’ duvar â†’ boÅŸluk â†’ Target
- [x] Bouncy ile yÃ¶n ver, Ice ile hÄ±zlandÄ±r, Sticky ile durdur
- [x] KullanÄ±labilir ink: **Ice, Sticky, Bouncy** â€” ilk kombine puzzle
- [x] Ink bÃ¼tÃ§esi: 100

**Level 05 â€” Labirent BaÅŸlangÄ±cÄ±**
- [x] Basit labirent, 2-3 dÃ¶nÃ¼ÅŸ
- [x] Hazard'lar dar geÃ§itlerde
- [x] KullanÄ±labilir ink: **Bouncy, Sticky**
- [x] Ink bÃ¼tÃ§esi: 80

**Level 06 â€” HÄ±z TuzaÄŸÄ±**
- [x] Uzun dÃ¼z koridor + sonunda hazard
- [x] Ice ile hÄ±zlanma â†’ Sticky ile frenleme zamanlamasÄ±
- [x] KullanÄ±labilir ink: **Ice, Sticky** â€” zamanlama Ã¶ÄŸretisi
- [x] Ink bÃ¼tÃ§esi: 60

**Level 07 â€” Pinball KÃ¶ÅŸeleri**
- [x] Kare arena, Target ortada (eriÅŸilmesi zor)
- [x] Bouncy duvarlarla sekme hesaplama
- [x] KullanÄ±labilir ink: **Bouncy, Ice**
- [x] Ink bÃ¼tÃ§esi: 70

**Level 08 â€” Dar GeÃ§it**
- [x] Dar koridor + hazard'lÄ± bÃ¶lge
- [x] Hassas Sticky kullanÄ±mÄ±
- [x] KullanÄ±labilir ink: **Sticky, Bouncy**
- [x] Ink bÃ¼tÃ§esi: **50** (kÄ±sÄ±tlÄ±)

**Level 09 â€” Spiral**
- [x] Spiral ÅŸekil, Ball dÄ±ÅŸtan iÃ§e
- [x] KullanÄ±labilir ink: **Ice, Sticky, Bouncy** â€” hepsi gerekli
- [x] Ink bÃ¼tÃ§esi: 60
- [x] YÃ¼ksek zorluk

**Level 10 â€” Final**
- [x] BÃ¼yÃ¼k harita, birden fazla yol
- [x] TÃ¼m mekanikler devrede
- [x] KullanÄ±labilir ink: **Ice, Sticky, Bouncy**
- [x] Ink bÃ¼tÃ§esi: 80
- [x] En az ink ile Ã§Ã¶zÃ¼m Ã¶dÃ¼llendirilir (yÄ±ldÄ±z sistemi)

### Level GeÃ§iÅŸ Sistemi
- [x] GameManager'a LevelManager entegrasyonu
- [x] Win â†’ Next buton â†’ `LevelManager.NextLevel()`
- [x] Fail â†’ Retry â†’ `LevelManager.RestartLevel()`
- [x] **"Game Complete" ekranÄ±:** Level 10 sonrasÄ± tebrik paneli gÃ¶ster (basit UI)
- [x] PlayerPrefs ile ilerleme kaydetme: `LastUnlockedLevel` key
- [x] LevelDefinition'daki `availableInks` alanÄ±na gÃ¶re UI'da sadece o level'Ä±n ink butonlarÄ±nÄ± gÃ¶ster

### DoÄŸrulama
- [x] 10 level sÄ±rasÄ±yla yÃ¼klenip oynanÄ±yor
- [x] Her level'da Ball spawn doÄŸru pozisyonda
- [x] Her level'da Target eriÅŸilebilir (Ã§Ã¶zÃ¼lebilir)
- [x] Level geÃ§iÅŸleri sorunsuz
- [x] Retry her level'da Ã§alÄ±ÅŸÄ±yor

---

## Step 10 â€” Audio: Ses Efektleri
**Branch:** `feature/step10-audio`

### AudioManager Script
- [ ] `Assets/_Game/Scripts/Audio/AudioManager.cs` oluÅŸtur
- [ ] Singleton pattern
- [ ] `AudioSource` bileÅŸeni (veya birden fazla â€” SFX + UI)
- [ ] `PlaySFX(AudioClip clip)` metodu
- [ ] Ses referanslarÄ±: `[SerializeField]` ile inspector'dan atanacak

### Ses EÅŸleÅŸtirmeleri
- [ ] Ã‡izim baÅŸlangÄ±cÄ±: `scratch_003.ogg`
- [ ] Ã‡izim bitiÅŸi: `drop_002.ogg`
- [ ] Ink seÃ§im deÄŸiÅŸikliÄŸi: `switch_003.ogg`
- [ ] Ball-Ã§izgi Ã§arpÄ±ÅŸmasÄ± (Bouncy): `glass_002.ogg`
- [ ] Win: `confirmation_002.ogg`
- [ ] Fail: `error_004.ogg`
- [ ] Buton tÄ±klama: `click_003.ogg`
- [ ] Level baÅŸlangÄ±cÄ±: `maximize_003.ogg`

### Entegrasyon
- [ ] DrawSystem â†’ Ã§izim baÅŸlangÄ±Ã§/bitiÅŸ sesi
- [ ] InkInventory â†’ ink deÄŸiÅŸim sesi
- [ ] GameManager â†’ Win/Fail sesleri
- [ ] UIHudController â†’ buton tÄ±klama sesleri
- [ ] BallController â†’ Bouncy Ã§arpÄ±ÅŸma sesi (opsiyonel)

### DoÄŸrulama
- [ ] TÃ¼m ses efektleri doÄŸru zamanda Ã§alÄ±yor
- [ ] Sesler Ã¼st Ã¼ste bindiÄŸinde bozulma yok
- [ ] Ses seviyesi dengeli

---

## Step 11 â€” Mobil Optimizasyon ve Son Polish
**Branch:** `feature/step11-polish`

### Mobil Input
- [ ] Touch input test: `Input.GetTouch(0)` veya Input System touch
- [ ] Multi-touch engelleme (sadece tek parmak Ã§izim)
- [ ] Touch â†’ mouse simÃ¼lasyonu editÃ¶rde Ã§alÄ±ÅŸÄ±yor

### Performans
- [ ] Profiler ile GC allocation kontrolÃ¼
- [ ] Pooling Ã§alÄ±ÅŸÄ±yor (Instantiate/Destroy Ã§aÄŸrÄ±sÄ± yok oyun iÃ§inde)
- [ ] `maxActiveLines` limiti aktif (30)
- [ ] `maxPoints` limiti aktif (60/Ã§izgi)
- [ ] Frame rate stabil (60 FPS hedef)

### VfxManager Script (Opsiyonel MVP)
- [ ] `Assets/_Game/Scripts/Vfx/VfxManager.cs` oluÅŸtur
- [ ] Singleton pattern
- [ ] Win efekti: `ParticlePack` â†’ konfeti/Ä±ÅŸÄ±k patlamasÄ±
- [ ] Fail efekti: kÃ¼Ã§Ã¼k patlama/duman
- [ ] Ball-Ã§izgi temasÄ±: kÃ¼Ã§Ã¼k splash (ink renginde)
- [ ] `PlayVfx(VfxType type, Vector3 position)` metodu

### GÃ¶rsel Polish
- [ ] Ball'a hafif trail efekti (TrailRenderer)
- [ ] Target pulse animasyonu (scale veya glow)
- [ ] Ink Ã§izgilerinde kalÄ±nlÄ±k varyasyonu (baÅŸlangÄ±Ã§/bitiÅŸ incelmesi â€” LineRenderer widthCurve)
- [ ] Win/Fail ekranÄ±nda basit parÃ§acÄ±k efekti (Particle Pack'ten)
- [ ] Duvar koyu, zemin aÃ§Ä±k renk kontrastÄ± (okunabilirlik)

### Build AyarlarÄ±
- [ ] Platform: Android/iOS
- [ ] Resolution: Portrait veya Landscape (GDD'ye gÃ¶re â€” top-down â†’ landscape Ã¶nerilir)
- [ ] Quality Settings mobil iÃ§in optimize
- [ ] Splash screen ayarlarÄ±

### Son Kontrol Listesi (Kabul Kriterleri)
- [ ] Ball level baÅŸlar baÅŸlamaz otomatik hareket ediyor
- [ ] Ã‡izim tek parmakla pÃ¼rÃ¼zsÃ¼z; minPointDist ile segment patlamÄ±yor
- [ ] 3 ink birbirinden belirgin: Ice kaydÄ±rÄ±yor, Sticky frenliyor, Bouncy sektiriyor
- [ ] Ã‡izgiler 5â€“10 sn sonra fade olup siliniyor; collider kapanÄ±yor
- [ ] Pooling Ã§alÄ±ÅŸÄ±yor (GC spike yok / minimal)
- [ ] Win: Target'a girince state Win, UI Next aÃ§Ä±lÄ±yor
- [ ] Fail: hazard/out-of-bounds state Fail, Retry Ã§alÄ±ÅŸÄ±yor
- [ ] 10 level oynanabilir
- [ ] Mobilde stabil performans

---

## Ã–zet Zaman Ã‡izelgesi

| AdÄ±m | Branch | Kapsam | BaÄŸÄ±mlÄ±lÄ±k |
|---|---|---|---|
| Step 1 | `feature/step1-bootstrap` | Proje yapÄ±sÄ±, GameManager, GameConfig | â€” |
| Step 2 | `feature/step2-ball-target` | Ball auto-move, Target, Win | Step 1 |
| Step 3 | `feature/step3-draw-system` | Ã‡izim mekaniÄŸi, InkLine | Step 2 |
| Step 4 | `feature/step4-collider-polish` | Collider doÄŸruluÄŸu, segment limiti | Step 3 |
| Step 5 | `feature/step5-ink-types` | 3 ink tipi, PhysicsMaterial2D, UI seÃ§im | Step 4 |
| Step 6 | `feature/step6-lifetime-pooling` | Fade, lifetime, object pooling | Step 5 |
| Step 7 | `feature/step7-fail-conditions` | Hazard, out-of-bounds, Fail state | Step 6 |
| Step 8 | `feature/step8-ui-hud` | Tam UI/HUD sistemi | Step 7 |
| Step 9 | `feature/step9-levels` | LevelManager, 10 level tasarÄ±mÄ± | Step 8 |
| Step 10 | `feature/step10-audio` | Ses efektleri entegrasyonu | Step 9 |
| Step 11 | `feature/step11-polish` | VfxManager, mobil optimizasyon, gÃ¶rsel polish, final test | Step 10 |

---

## Teslim Kontrol Listesi (AGENT.md BÃ¶lÃ¼m 12)

Her step tamamlandÄ±ÄŸÄ±nda ÅŸu Ã§Ä±ktÄ±lar doÄŸrulanmalÄ±:

- [ ] **Proje tree:** hangi klasÃ¶rde ne var (gÃ¼ncel)
- [ ] **Script listesi:** oluÅŸturulan / deÄŸiÅŸtirilen script'ler
- [ ] **Kurulum adÄ±mlarÄ±:** scene/prefab baÄŸlamalarÄ± (inspector referanslar)
- [ ] **OynanÄ±ÅŸ test adÄ±mlarÄ±:** Level 01'den 10'a nasÄ±l test edilir
- [ ] **Bilinen riskler / TODO'lar:** MVP dÄ±ÅŸÄ± kalan maddeler

---

## MVP SonrasÄ± (Backlog)

- [ ] Rewarded reklam: +ink / undo / hint
- [ ] Interstitial reklam pacing
- [ ] Gate / button / moving obstacle mekanikleri
- [ ] Tutorial pop-up'lar (ilk 3 level)
- [ ] MÃ¼zik ekleme
- [ ] Haptic feedback (mobil)
- [ ] Analytics entegrasyonu
- [ ] Leaderboard / yÄ±ldÄ±z sistemi
- [ ] Daha fazla level (20+)
- [ ] Tema/skin sistemi
