# Tri‑Ink Track — Geliştirme Yol Haritası

> **Proje:** Tri‑Ink Track (Üç Mürekkep Parkuru)
> **Engine:** Unity 6000.3.9f1 — URP 2D
> **Platform:** iOS / Android (Mobil)
> **Tarih:** 2026-02-25

---

## Mevcut Proje Durumu

| Kategori | Durum |
|---|---|
| Unity Projesi | Kurulu (URP 2D, Input System) |
| Sahne | `SampleScene.unity` — Kamera + Global Light 2D |
| Sprite'lar | Kenney Physics Assets, Rolling Ball Assets (~500+ sprite) |
| UI Elementleri | Kenney UI Pack (butonlar, checkbox'lar, slider'lar) |
| Fontlar | Kenney Future, Kenney Future Narrow |
| Ses Efektleri | Kenney Interface Sounds (100+ OGG) |
| Parçacık Efektleri | Unity Particle Pack (örnek) |
| Oyun Script'leri | **YOK — Sıfırdan yazılacak** |
| Fizik Materyalleri | **YOK — Oluşturulacak** |
| Prefab'lar | **YOK — Oluşturulacak** |
| Level Tasarımları | **YOK — Tasarlanacak** |

---

## Hedef Klasör Yapısı

```
Assets/
├── _Game/
│   ├── Art/
│   │   ├── Sprites/          ← Ball, Target, Hazard, Ink görselleri
│   │   └── Backgrounds/      ← Level arka planları
│   ├── Audio/
│   │   ├── SFX/              ← Oyun sesleri (çizim, çarpma, win, fail)
│   │   └── Music/            ← (MVP sonrası)
│   ├── Fonts/                ← Kenney Future fontları (referans)
│   ├── PhysicsMaterials/     ← Ice, Sticky, Bouncy
│   ├── Prefabs/
│   │   ├── Ball.prefab
│   │   ├── Target.prefab
│   │   ├── InkLine.prefab
│   │   ├── Hazard.prefab
│   │   ├── Wall.prefab
│   │   └── Levels/
│   │       ├── Level_01.prefab … Level_10.prefab
│   ├── Scenes/
│   │   └── GameScene.unity
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs
│   │   │   └── LevelManager.cs
│   │   ├── Ball/
│   │   │   └── BallController.cs
│   │   ├── Drawing/
│   │   │   ├── DrawSystem.cs
│   │   │   ├── InkLine.cs
│   │   │   ├── InkLineLifetime.cs
│   │   │   └── InkLinePool.cs
│   │   ├── Ink/
│   │   │   ├── InkType.cs           (enum)
│   │   │   └── InkInventory.cs
│   │   ├── UI/
│   │   │   └── UIHudController.cs
│   │   ├── Audio/
│   │   │   └── AudioManager.cs
│   │   ├── Vfx/
│   │   │   └── VfxManager.cs       (opsiyonel MVP)
│   │   └── Config/
│   │       ├── GameConfig.cs         (ScriptableObject)
│   │       └── LevelDefinition.cs    (ScriptableObject)
│   ├── ScriptableObjects/
│   │   ├── GameConfig.asset
│   │   └── Levels/
│   │       ├── Level_01_Def.asset … Level_10_Def.asset
│   └── UI/
│       ├── HUD.prefab
│       └── Sprites/          ← UI ikonları (ink seçim, retry, vb.)
├── kenney_physics-assets/     ← (Mevcut)
├── kenney_rolling-ball-assets/ ← (Mevcut)
├── kenney_ui-pack/            ← (Mevcut)
├── kenney_interface-sounds/   ← (Mevcut)
└── Settings/                  ← (Mevcut — URP ayarları)
```

---

## Kullanılacak Mevcut Asset Eşleştirmesi

| Oyun Elementi | Kaynak Asset | Dosya Yolu |
|---|---|---|
| Bilye (Ball) | `ball_blue_large.png` | `kenney_rolling-ball-assets/PNG/Default/ball_blue_large.png` |
| Hedef (Target) | `hole_end.png` | `kenney_rolling-ball-assets/PNG/Default/hole_end.png` |
| Başlangıç | `hole_start.png` | `kenney_rolling-ball-assets/PNG/Default/hole_start.png` |
| Duvar blokları | `block_large.png`, `block_narrow.png` | `kenney_rolling-ball-assets/PNG/Default/block_*.png` |
| Sabit engeller | `locked_*` | `kenney_rolling-ball-assets/PNG/Default/locked_*.png` |
| Arka plan | `background_blue.png` vb. | `kenney_rolling-ball-assets/PNG/Default/background_*.png` |
| Yıldız (skor) | `star.png`, `star_outline.png` | `kenney_rolling-ball-assets/PNG/Default/star*.png` |
| UI butonları | `button_rectangle_flat.png` | `kenney_ui-pack/PNG/*/button_rectangle_*.png` |
| Ink seçim ikonları | `icon_circle.png` (renklendirilmiş) | `kenney_ui-pack/PNG/*/icon_circle*.png` |
| Checkbox | `checkbox_*.png` | `kenney_ui-pack/PNG/*/checkbox_*.png` |
| Retry/Next buton | `button_round_depth_flat.png` | `kenney_ui-pack/PNG/*/button_round_*.png` |
| Font | Kenney Future | `kenney_ui-pack/Fonts/Kenney Future.ttf` |
| Tıklama sesi | `click_003.ogg` | `kenney_interface-sounds/Audio/click_003.ogg` |
| Çizim sesi | `scratch_003.ogg` | `kenney_interface-sounds/Audio/scratch_003.ogg` |
| Win sesi | `confirmation_002.ogg` | `kenney_interface-sounds/Audio/confirmation_002.ogg` |
| Fail sesi | `error_004.ogg` | `kenney_interface-sounds/Audio/error_004.ogg` |
| Buton hover | `select_002.ogg` | `kenney_interface-sounds/Audio/select_002.ogg` |
| Ink seçim sesi | `switch_003.ogg` | `kenney_interface-sounds/Audio/switch_003.ogg` |
| Bouncy sekme | `glass_002.ogg` | `kenney_interface-sounds/Audio/glass_002.ogg` |

---

## Script Listesi ve Sorumlulukları

| Script | Sorumluluk |
|---|---|
| `GameManager.cs` | Game state yönetimi (Ready → Playing → Win/Fail → Retry/Next) |
| `LevelManager.cs` | Level yükleme, sıfırlama, ilerleme kaydetme |
| `BallController.cs` | Rigidbody2D auto-move, hız normalizasyonu, reset |
| `DrawSystem.cs` | Touch/mouse input → point sampling → InkLine spawn |
| `InkLine.cs` | LineRenderer + EdgeCollider2D senkronizasyonu |
| `InkLineLifetime.cs` | Timer, fade animasyonu, collider disable, pool'a iade |
| `InkLinePool.cs` | Object pooling (ink tipi başına) |
| `InkType.cs` | `enum InkType { Ice, Sticky, Bouncy }` |
| `InkInventory.cs` | Ink puanı tüketimi ve kalan miktar takibi |
| `UIHudController.cs` | Ink seçim butonları, ink bar, retry/next UI |
| `AudioManager.cs` | SFX tetikleme (singleton) |
| `GameConfig.cs` | ScriptableObject — global oyun parametreleri |
| `LevelDefinition.cs` | ScriptableObject — level başına konfigürasyon |
| `VfxManager.cs` | Basit VFX tetikleme — win/fail parçacık efektleri (opsiyonel MVP) |

---

## Kod Standartları (Referans)

> AGENT.md Bölüm 11'den — tüm adımlarda uygulanacak kurallar.

- Public API minimize: `[SerializeField] private` tercih et
- `Update`/`FixedUpdate` içinde allocation yapma (no `new`, no `string +`)
- Pooling: `Instantiate`/`Destroy` döngüsünden kaçın
- Çizgi noktaları için `List<Vector3>` yeniden kullan (`Clear()` + reuse)
- Parametreleri `ScriptableObject` veya `[Serializable]` config struct ile tek yerde topla
- Debug logları `#if UNITY_EDITOR` ile kontrol et

---

# ADIMLAR

---

## Step 1 — Bootstrap: Proje Altyapısı ve GameScene
**Branch:** `feature/step1-bootstrap`

### Klasör ve Sahne Hazırlığı
- [x] `Assets/_Game/` ana klasör yapısını oluştur (Art, Audio, Scripts, Prefabs, Scenes, PhysicsMaterials, ScriptableObjects, UI)
- [x] `Assets/_Game/Scenes/GameScene.unity` oluştur (SampleScene içeriği temel alındı; SampleScene yerinde kaldı)
- [x] EditorBuildSettings'e GameScene'i ekle
- [x] Kamera ayarlarını doğrula: Orthographic, 2D, size = 10
- [x] Global Light 2D ayarlarını doğrula

### GameManager (State Machine)
- [x] `Assets/_Game/Scripts/Core/GameManager.cs` oluştur
- [x] `GameState` enum tanımla: `Ready, Playing, Win, Fail`
- [x] Singleton pattern uygula (DontDestroyOnLoad kullanma — tek sahne MVP)
- [x] State geçiş metodları: `StartGame()`, `OnWin()`, `OnFail()`, `Retry()`, `NextLevel()`
- [x] `OnGameStateChanged` event tanımla (diğer sistemlerin dinlemesi için)

### GameConfig (ScriptableObject)
- [x] `Assets/_Game/Scripts/Config/GameConfig.cs` oluştur
- [x] Temel parametreler: `ballSpeed`, `inkLifetime`, `maxActiveLines`, `maxPointsPerLine`, `minPointDist`, `totalInkPoints`
- [x] `Assets/_Game/ScriptableObjects/GameConfig.asset` oluştur

### Sahne Hiyerarşisi
- [x] GameScene'de boş GameObject'ler oluştur: `GameManager`, `LevelRoot`, `DrawSystem`, `UI Canvas`
- [x] GameManager script'ini GameManager objesine ata
- [x] GameConfig asset'ini GameManager'a referans olarak bağla

### Doğrulama
- [ ] Play modunda GameManager singleton erişilebilir
- [ ] State geçişleri Console'da loglanıyor
- [ ] Proje hatasız derleniyor

---

## Step 2 — Ball + Target: Temel Hareket ve Win Koşulu
**Branch:** `feature/step2-ball-target`

### Ball Prefab
- [x] `Assets/_Game/Prefabs/Ball.prefab` oluştur
- [x] Sprite: `kenney_rolling-ball-assets/PNG/Default/ball_blue_large.png`
- [x] Bileşenler: `SpriteRenderer`, `Rigidbody2D`, `CircleCollider2D`
- [x] Rigidbody2D ayarları: `gravityScale = 0`, `linearDrag = 0.1–0.4` (tweak), `angularDrag = 0.5`, `collisionDetection = Continuous`
- [x] Tag: `Ball`

### BallController Script
- [x] `Assets/_Game/Scripts/Ball/BallController.cs` oluştur
- [x] `[SerializeField] private float targetSpeed = 5f` (4–7 arası tweak)
- [x] `[SerializeField] private Vector2 initialDirection = Vector2.right`
- [x] `FixedUpdate` içinde hız normalizasyonu: `rb.velocity = rb.velocity.normalized * targetSpeed`
- [x] Çok düşük hız fallback'i: velocity.magnitude < 0.1f ise `initialDirection * targetSpeed` uygula
- [x] Bouncy çarpışma sonrası `maxSpeed` clamp (aşırı hız önleme)
- [x] `ResetBall()` metodu: pozisyon ve hız sıfırlama (level restart için)
- [x] Başlangıç pozisyonu kaydetme (`spawnPosition`)
- [x] Level prefab'ından spawn pozisyonu ve başlangıç yönü okuma

### Target Prefab
- [x] `Assets/_Game/Prefabs/Target.prefab` oluştur
- [x] Sprite: `kenney_rolling-ball-assets/PNG/Default/hole_small_end.png`
- [x] Bileşenler: `SpriteRenderer`, `CircleCollider2D (isTrigger = true)`
- [x] Basit pulse animasyonu (scale ping-pong, script veya Animation)

### Win Trigger
- [x] Target prefab'a `OnTriggerEnter2D` ile Ball teması algılama ekle (Tag: "Ball")
- [x] Temas olduğunda `GameManager.Instance.OnWin()` çağır
- [x] Ball'a "Ball" tag'i ata

### Wall Prefab
- [x] `Assets/_Game/Prefabs/Wall.prefab` oluştur
- [x] Sprite: `kenney_rolling-ball-assets/PNG/Default/block_large.png` (tiling)
- [x] Bileşen: `BoxCollider2D`
- [x] Farklı boyutlar için ölçeklenebilir yapı

### Test Level Düzeni
- [x] GameScene'de basit dikdörtgen alan: 4 duvar + Ball + Target
- [x] Ball sol tarafta, Target sağ tarafta
- [x] Play modunda Ball otomatik hareket ediyor ve duvarlara çarpıyor

### Doğrulama
- [x] Ball otomatik hareket ediyor, hızı sabit kalıyor
- [x] Duvarlara çarpınca sekmesi doğal
- [x] Target'a ulaşınca Win state tetikleniyor
- [x] Console'da Win log'u görünüyor

---

## Step 3 — DrawSystem v1: Çizim Mekaniği
**Branch:** `feature/step3-draw-system`

### InkType Enum
- [x] `Assets/_Game/Scripts/Ink/InkType.cs` oluştur
- [x] `public enum InkType { Ice, Sticky, Bouncy }`

### DrawSystem Script
- [x] `Assets/_Game/Scripts/Drawing/DrawSystem.cs` oluştur
- [x] Mouse/Touch input algılama (Input System veya legacy — MVP için legacy tercih)
- [x] `Camera.main.ScreenToWorldPoint()` ile screen → world dönüşümü
- [x] Nokta örnekleme: yeni nokta ancak `minPointDist` (0.15) uzaklıkta ise ekle
- [x] `isDrawing` flag ile pointer down/move/up yönetimi
- [x] UI üstüne çizim engelleme: `EventSystem.current.IsPointerOverGameObject()` kontrolü
- [x] Mobil touch için: `EventSystem.current.IsPointerOverGameObject(touch.fingerId)`

### InkLine Script
- [x] `Assets/_Game/Scripts/Drawing/InkLine.cs` oluştur
- [x] `LineRenderer` referansı ve ayarları (width, color, material)
- [x] `EdgeCollider2D` referansı
- [x] `AddPoint(Vector3 worldPos)` metodu
- [x] LineRenderer güncelleme: `SetPositionCount()`, `SetPosition()`
- [x] EdgeCollider2D sync: world → local dönüşümü ile `points` dizisi güncelle
- [x] `maxPoints` limiti (60)

### InkLine Prefab
- [x] `Assets/_Game/Prefabs/InkLine.prefab` oluştur
- [x] Bileşenler: `LineRenderer`, `EdgeCollider2D`, `InkLine` script
- [x] LineRenderer ayarları: width 0.08–0.12, Sprites-Default material
- [x] EdgeCollider2D: başlangıçta boş

### Çizim Akışı
- [x] Pointer Down → yeni InkLine instantiate (veya pool'dan al)
- [x] Pointer Move → `InkLine.AddPoint()`
- [x] Pointer Up → çizgiyi "kilitle" (çizim durur, ömür geri sayımı başlar)
- [x] Minimum 2 nokta yoksa çizgiyi sil/iade et

### Doğrulama
- [x] Mouse ile sürükleyince pürüzsüz çizgi oluşuyor
- [x] Çizgi kalınlığı tutarlı
- [x] Ball çizgiye çarpıyor (EdgeCollider2D çalışıyor)
- [x] UI butonlarına tıklayınca çizim başlamıyor

---

## Step 4 — EdgeCollider2D Doğruluğu ve Çizgi İyileştirmesi
**Branch:** `feature/step4-collider-polish`

### Collider Senkronizasyonu
- [x] `InkLine.cs` — EdgeCollider2D güncelleme optimizasyonu
- [x] World → Local dönüşümü doğrulaması: `transform.InverseTransformPoint()`
- [x] Çizgi objesinin pozisyonu (0,0,0) olmalı veya offset hesaplanmalı
- [x] Collider noktalarının LineRenderer ile birebir eşleştiğini doğrula

### Segment Limiti
- [x] `maxPoints` aşılınca çizimi otomatik bitir (pointer up simüle)
- [x] Toplam aktif çizgi sayısı kontrolü: `maxActiveLines` (30)
- [x] Limit aşılınca en eski çizgiyi kaldır veya yeni çizim engelle

### Çizim Kalitesi
- [x] `minPointDist` parametresini GameConfig'den oku
- [x] Çok kısa çizgileri (< 3 nokta) otomatik sil
- [x] LineRenderer corner vertices ve end cap ayarları (Round)

### Doğrulama
- [x] Collider tam olarak çizginin üstünde
- [x] Ball çizginin her noktasında çarpışıyor
- [x] Segment patlaması yok (çok fazla nokta oluşmuyor)
- [x] 30+ çizgi çizilince sistem stabil

---

## Step 5 — Ink Tipleri ve Fizik Materyalleri
**Branch:** `feature/step5-ink-types`

### PhysicsMaterial2D Oluşturma
- [x] `Assets/_Game/PhysicsMaterials/Ice.physicsMaterial2D` — friction: 0.02, bounciness: 0.05
- [x] `Assets/_Game/PhysicsMaterials/Sticky.physicsMaterial2D` — friction: 0.9, bounciness: 0
- [x] `Assets/_Game/PhysicsMaterials/Bouncy.physicsMaterial2D` — friction: 0.1, bounciness: 0.9

### InkLine'a Tip Atama
- [x] `InkLine.cs` — `InkType currentType` alanı ekle
- [x] `SetInkType(InkType type)` metodu: EdgeCollider2D'ye uygun PhysicsMaterial2D ata
- [x] Her ink tipi için farklı renk: Ice → Mavi (#00BFFF), Sticky → Turuncu (#FF8C00), Bouncy → Yeşil (#32CD32)
- [x] LineRenderer rengini ink tipine göre ayarla

### InkInventory Script
- [x] `Assets/_Game/Scripts/Ink/InkInventory.cs` oluştur
- [x] `[SerializeField] private int totalInkPoints = 100`
- [x] `ConsumeInk(int amount)` — çizim sırasında her segment için puan düş
- [x] `HasInk()` — kontrol
- [x] `ResetInk()` — level restart
- [x] `OnInkChanged` event (UI güncelleme için)
- [x] Seçili ink tipi takibi: `currentInkType`

### DrawSystem — Ink Entegrasyonu
- [x] DrawSystem'e `InkInventory` referansı ekle
- [x] Çizim sırasında ink puanı tüket (her yeni nokta = 1 puan)
- [x] Ink bitince çizimi durdur
- [x] Yeni InkLine spawn'larken seçili ink tipini ata

### UI — Ink Seçim Butonları (Temel)
- [x] UI Canvas'a 3 buton ekle (alt bar): Ice / Sticky / Bouncy
- [x] Buton sprite'ları: `kenney_ui-pack` butonları (mavi, turuncu, yeşil)
- [x] Seçili buton vurgusu (scale veya border)
- [x] Buton tıklama → `InkInventory.currentInkType` değiştir

### Doğrulama
- [x] 3 farklı renkle çizgi çizilebiliyor
- [x] Ice çizgisinde Ball kayıyor (düşük sürtünme)
- [x] Sticky çizgisinde Ball yavaşlıyor/duruyor
- [x] Bouncy çizgisinde Ball belirgin şekilde sekiyor
- [x] Ink puanı azalıyor, bitince çizim duruyor
- [x] UI'dan ink tipi değiştirilebiliyor

---

## Step 6 — Lifetime, Fade ve Object Pooling
**Branch:** `feature/step6-lifetime-pooling`

### InkLineLifetime Script
- [x] `Assets/_Game/Scripts/Drawing/InkLineLifetime.cs` oluştur
- [x] `lifeSeconds` (5–10 sn, GameConfig'den oku)
- [x] `fadeDuration` (0.5 sn)
- [x] `spawnTime` kaydı
- [x] Ömür dolunca fade başlat: LineRenderer alpha 1→0 (Coroutine veya Update)
- [x] Fade bitince EdgeCollider2D disable
- [x] Pool'a iade: `gameObject.SetActive(false)` + pool'a geri koy

### InkLinePool Script
- [x] `Assets/_Game/Scripts/Drawing/InkLinePool.cs` oluştur
- [x] `Queue<InkLine>` pool yapısı
- [x] `Get()` — pool'dan al veya yeni oluştur
- [x] `Return(InkLine line)` — sıfırla ve pool'a koy
- [x] Başlangıçta `initialPoolSize` (10) kadar pre-spawn
- [x] InkLine sıfırlama: pozisyon, nokta sayısı, renk, collider temizle

### DrawSystem — Pool Entegrasyonu
- [x] `Instantiate` yerine `InkLinePool.Get()` kullan
- [x] Çizgi ömrü dolunca otomatik pool'a dönüş
- [x] Aktif çizgi sayısı takibi

### Performans Kontrolleri
- [x] `Update`/`FixedUpdate` içinde allocation olmaması
- [x] `List<Vector3>` yeniden kullanımı (clear + reuse)
- [x] Profiler ile GC spike kontrolü

### Doğrulama
- [x] Çizgiler 5–10 sn sonra fade oluyor
- [x] Fade sırasında görsel yumuşak geçiş
- [x] Collider fade bitince kapanıyor (Ball artık geçiyor)
- [x] Aynı çizgiyi tekrar çizince pool'dan alınıyor (Instantiate yok)
- [x] 50+ çizgi çizip silme döngüsünde performans stabil

---

## Step 7 — Fail Koşulları ve Sınır Sistemi
**Branch:** `feature/step7-fail-conditions`

### Hazard Prefab
- [x] `Assets/_Game/Prefabs/Hazard.prefab` oluştur
- [x] Sprite: `kenney_physics-assets/PNG/Glass Elements/` veya kırmızı tint uygulanmış blok
- [x] Bileşenler: `SpriteRenderer`, `BoxCollider2D (isTrigger = true)`
- [x] "Hazard" tag'i ata

### Out-of-Bounds Algılama
- [x] Kamera sınırlarını hesapla veya sabit sınır collider'ları oluştur
- [x] 4 kenar trigger (ekran dışı) — büyük BoxCollider2D (isTrigger)
- [x] "Boundary" tag'i ata

### BallController — Fail Tetikleme
- [x] `OnTriggerEnter2D` — "Hazard" veya "Boundary" tag'i kontrolü
- [x] Temas → `GameManager.Instance.OnFail()`

### GameManager — Fail State
- [x] `OnFail()` → state = Fail, event tetikle
- [x] Ball hareketi durdur (veya kinematic yap)
- [x] Retry butonu aktif

### Doğrulama
- [x] Ball hazard'a değince Fail state
- [x] Ball ekran dışına çıkınca Fail state
- [x] Fail durumunda Ball duruyor
- [x] Console'da Fail log'u

---

## Step 8 — UI/HUD: Tam Arayüz
**Branch:** `feature/step8-ui-hud`

### UIHudController Script
- [x] `Assets/_Game/Scripts/UI/UIHudController.cs` oluştur
- [x] GameManager state event'lerini dinle
- [x] Panelleri state'e göre göster/gizle

### HUD Elemanları
- [x] **Ink Seçim Bar (Alt):** 3 buton — her biri ink rengiyle, seçili olan büyük/parlak
- [x] **Ink Miktarı Göstergesi:** Slider veya fill bar — kalan ink puanı
- [x] **Retry Butonu:** Sağ üst köşe, her zaman görünür
- [x] **Pause Butonu (Opsiyonel):** Sol üst köşe — oyunu duraklat, devam et, ana menü
- [x] **Win Paneli:** "Level Complete!" + Next Level butonu + yıldız gösterimi
- [x] **Fail Paneli:** "Failed!" + Retry butonu
- [x] **Game Complete Paneli:** Son level sonrası tebrik ekranı

### UI Sprite Atamaları
- [x] Butonlar: `kenney_ui-pack/PNG/Blue/button_rectangle_flat.png` (ve renk varyantları)
- [x] Retry ikon: `kenney_ui-pack/PNG/Grey/icon_circle.png` (R harfi overlay)
- [x] Next ikon: `kenney_ui-pack/PNG/Green/Default/arrow_basic_e.png`
- [x] Star: `kenney_rolling-ball-assets/PNG/Default/star.png`
- [x] Font: `kenney_ui-pack/Font/Kenney Future.ttf`

### UI Fonksiyonelliği
- [x] Retry butonu → `GameManager.Retry()` → level sıfırla, ink sıfırla, ball sıfırla
- [x] Next butonu → `GameManager.NextLevel()` → sonraki level yükle
- [x] Ink butonları → `InkInventory.currentInkType` değiştir
- [x] Ink bar → `InkInventory.OnInkChanged` event'ini dinle, fill güncelle

### State'e Göre UI
- [x] **Ready/Playing:** HUD görünür, Win/Fail panelleri gizli
- [x] **Win:** Win paneli göster, HUD butonları devre dışı
- [x] **Fail:** Fail paneli göster, HUD butonları devre dışı

### Doğrulama
- [x] Tüm butonlar çalışıyor
- [x] Ink bar doğru güncelleniyor
- [x] Win/Fail panelleri doğru zamanda görünüyor
- [x] Retry level'ı tamamen sıfırlıyor
- [x] UI üstünde çizim başlamıyor

---

## Step 9 — Level Sistemi ve 10 Level Tasarımı
**Branch:** `feature/step9-levels`

### LevelManager Script
- [ ] `Assets/_Game/Scripts/Core/LevelManager.cs` oluştur
- [ ] Level prefab listesi: `[SerializeField] private GameObject[] levelPrefabs`
- [ ] `LoadLevel(int index)` — mevcut level'ı sil, yenisini `LevelRoot` altına instantiate et
- [ ] `currentLevelIndex` takibi
- [ ] `RestartLevel()` — aynı level'ı yeniden yükle
- [ ] `NextLevel()` — index++, yükle
- [ ] Son level sonrası: başa dön veya "Game Complete" göster

### LevelDefinition (ScriptableObject)
- [ ] `Assets/_Game/Scripts/Config/LevelDefinition.cs` oluştur
- [ ] Alanlar: `levelName`, `levelPrefab`, `availableInks` (Ice/Sticky/Bouncy hangilerini kullanabilir), `inkBudget`, `targetTime` (opsiyonel)
- [ ] 10 adet asset oluştur: `Level_01_Def.asset` … `Level_10_Def.asset`

### Level Prefab Yapısı
Her level prefab şunları içerir:
- [ ] Ball spawn noktası (Transform marker)
- [ ] Target pozisyonu
- [ ] Duvarlar (BoxCollider2D)
- [ ] Hazard'lar (isTrigger)
- [ ] Arka plan sprite

### 10 Level Tasarımı

**Level 01 — Düz Yol (Tutorial: Çizim Öğretici)**
- [ ] Basit koridor, Ball solda → Target sağda
- [ ] Engel yok, sadece çizim pratiği
- [ ] Kullanılabilir ink: **Sticky** — fren etkisini göster
- [ ] Ink bütçesi: 100 (cömert)
- [ ] Ball başlangıç yönü: → (sağa)

**Level 02 — İlk Dönüş (Bouncy Tanıtım)**
- [ ] L şeklinde koridor
- [ ] Ball düz gidiyor, köşeyi dönmesi gerek
- [ ] Kullanılabilir ink: **Bouncy** — yön değiştirme öğretisi
- [ ] Ink bütçesi: 80
- [ ] Ball başlangıç yönü: → (sağa)

**Level 03 — Kaygan Zemin (Ice Tanıtım)**
- [ ] Geniş alan, Ball yavaş başlıyor
- [ ] Ice çizgisiyle hızlandırma
- [ ] Kullanılabilir ink: **Ice** — hızlandırma öğretisi
- [ ] Ink bütçesi: 80
- [ ] Target uzak köşede

**Level 04 — Üçünü Birleştir**
- [ ] Ball → duvar → boşluk → Target
- [ ] Bouncy ile yön ver, Ice ile hızlandır, Sticky ile durdur
- [ ] Kullanılabilir ink: **Ice, Sticky, Bouncy** — ilk kombine puzzle
- [ ] Ink bütçesi: 100

**Level 05 — Labirent Başlangıcı**
- [ ] Basit labirent, 2-3 dönüş
- [ ] Hazard'lar dar geçitlerde
- [ ] Kullanılabilir ink: **Bouncy, Sticky**
- [ ] Ink bütçesi: 80

**Level 06 — Hız Tuzağı**
- [ ] Uzun düz koridor + sonunda hazard
- [ ] Ice ile hızlanma → Sticky ile frenleme zamanlaması
- [ ] Kullanılabilir ink: **Ice, Sticky** — zamanlama öğretisi
- [ ] Ink bütçesi: 60

**Level 07 — Pinball Köşeleri**
- [ ] Kare arena, Target ortada (erişilmesi zor)
- [ ] Bouncy duvarlarla sekme hesaplama
- [ ] Kullanılabilir ink: **Bouncy, Ice**
- [ ] Ink bütçesi: 70

**Level 08 — Dar Geçit**
- [ ] Dar koridor + hazard'lı bölge
- [ ] Hassas Sticky kullanımı
- [ ] Kullanılabilir ink: **Sticky, Bouncy**
- [ ] Ink bütçesi: **50** (kısıtlı)

**Level 09 — Spiral**
- [ ] Spiral şekil, Ball dıştan içe
- [ ] Kullanılabilir ink: **Ice, Sticky, Bouncy** — hepsi gerekli
- [ ] Ink bütçesi: 60
- [ ] Yüksek zorluk

**Level 10 — Final**
- [ ] Büyük harita, birden fazla yol
- [ ] Tüm mekanikler devrede
- [ ] Kullanılabilir ink: **Ice, Sticky, Bouncy**
- [ ] Ink bütçesi: 80
- [ ] En az ink ile çözüm ödüllendirilir (yıldız sistemi)

### Level Geçiş Sistemi
- [ ] GameManager'a LevelManager entegrasyonu
- [ ] Win → Next buton → `LevelManager.NextLevel()`
- [ ] Fail → Retry → `LevelManager.RestartLevel()`
- [ ] **"Game Complete" ekranı:** Level 10 sonrası tebrik paneli göster (basit UI)
- [ ] PlayerPrefs ile ilerleme kaydetme: `LastUnlockedLevel` key
- [ ] LevelDefinition'daki `availableInks` alanına göre UI'da sadece o level'ın ink butonlarını göster

### Doğrulama
- [ ] 10 level sırasıyla yüklenip oynanıyor
- [ ] Her level'da Ball spawn doğru pozisyonda
- [ ] Her level'da Target erişilebilir (çözülebilir)
- [ ] Level geçişleri sorunsuz
- [ ] Retry her level'da çalışıyor

---

## Step 10 — Audio: Ses Efektleri
**Branch:** `feature/step10-audio`

### AudioManager Script
- [ ] `Assets/_Game/Scripts/Audio/AudioManager.cs` oluştur
- [ ] Singleton pattern
- [ ] `AudioSource` bileşeni (veya birden fazla — SFX + UI)
- [ ] `PlaySFX(AudioClip clip)` metodu
- [ ] Ses referansları: `[SerializeField]` ile inspector'dan atanacak

### Ses Eşleştirmeleri
- [ ] Çizim başlangıcı: `scratch_003.ogg`
- [ ] Çizim bitişi: `drop_002.ogg`
- [ ] Ink seçim değişikliği: `switch_003.ogg`
- [ ] Ball-çizgi çarpışması (Bouncy): `glass_002.ogg`
- [ ] Win: `confirmation_002.ogg`
- [ ] Fail: `error_004.ogg`
- [ ] Buton tıklama: `click_003.ogg`
- [ ] Level başlangıcı: `maximize_003.ogg`

### Entegrasyon
- [ ] DrawSystem → çizim başlangıç/bitiş sesi
- [ ] InkInventory → ink değişim sesi
- [ ] GameManager → Win/Fail sesleri
- [ ] UIHudController → buton tıklama sesleri
- [ ] BallController → Bouncy çarpışma sesi (opsiyonel)

### Doğrulama
- [ ] Tüm ses efektleri doğru zamanda çalıyor
- [ ] Sesler üst üste bindiğinde bozulma yok
- [ ] Ses seviyesi dengeli

---

## Step 11 — Mobil Optimizasyon ve Son Polish
**Branch:** `feature/step11-polish`

### Mobil Input
- [ ] Touch input test: `Input.GetTouch(0)` veya Input System touch
- [ ] Multi-touch engelleme (sadece tek parmak çizim)
- [ ] Touch → mouse simülasyonu editörde çalışıyor

### Performans
- [ ] Profiler ile GC allocation kontrolü
- [ ] Pooling çalışıyor (Instantiate/Destroy çağrısı yok oyun içinde)
- [ ] `maxActiveLines` limiti aktif (30)
- [ ] `maxPoints` limiti aktif (60/çizgi)
- [ ] Frame rate stabil (60 FPS hedef)

### VfxManager Script (Opsiyonel MVP)
- [ ] `Assets/_Game/Scripts/Vfx/VfxManager.cs` oluştur
- [ ] Singleton pattern
- [ ] Win efekti: `ParticlePack` → konfeti/ışık patlaması
- [ ] Fail efekti: küçük patlama/duman
- [ ] Ball-çizgi teması: küçük splash (ink renginde)
- [ ] `PlayVfx(VfxType type, Vector3 position)` metodu

### Görsel Polish
- [ ] Ball'a hafif trail efekti (TrailRenderer)
- [ ] Target pulse animasyonu (scale veya glow)
- [ ] Ink çizgilerinde kalınlık varyasyonu (başlangıç/bitiş incelmesi — LineRenderer widthCurve)
- [ ] Win/Fail ekranında basit parçacık efekti (Particle Pack'ten)
- [ ] Duvar koyu, zemin açık renk kontrastı (okunabilirlik)

### Build Ayarları
- [ ] Platform: Android/iOS
- [ ] Resolution: Portrait veya Landscape (GDD'ye göre — top-down → landscape önerilir)
- [ ] Quality Settings mobil için optimize
- [ ] Splash screen ayarları

### Son Kontrol Listesi (Kabul Kriterleri)
- [ ] Ball level başlar başlamaz otomatik hareket ediyor
- [ ] Çizim tek parmakla pürüzsüz; minPointDist ile segment patlamıyor
- [ ] 3 ink birbirinden belirgin: Ice kaydırıyor, Sticky frenliyor, Bouncy sektiriyor
- [ ] Çizgiler 5–10 sn sonra fade olup siliniyor; collider kapanıyor
- [ ] Pooling çalışıyor (GC spike yok / minimal)
- [ ] Win: Target'a girince state Win, UI Next açılıyor
- [ ] Fail: hazard/out-of-bounds state Fail, Retry çalışıyor
- [ ] 10 level oynanabilir
- [ ] Mobilde stabil performans

---

## Özet Zaman Çizelgesi

| Adım | Branch | Kapsam | Bağımlılık |
|---|---|---|---|
| Step 1 | `feature/step1-bootstrap` | Proje yapısı, GameManager, GameConfig | — |
| Step 2 | `feature/step2-ball-target` | Ball auto-move, Target, Win | Step 1 |
| Step 3 | `feature/step3-draw-system` | Çizim mekaniği, InkLine | Step 2 |
| Step 4 | `feature/step4-collider-polish` | Collider doğruluğu, segment limiti | Step 3 |
| Step 5 | `feature/step5-ink-types` | 3 ink tipi, PhysicsMaterial2D, UI seçim | Step 4 |
| Step 6 | `feature/step6-lifetime-pooling` | Fade, lifetime, object pooling | Step 5 |
| Step 7 | `feature/step7-fail-conditions` | Hazard, out-of-bounds, Fail state | Step 6 |
| Step 8 | `feature/step8-ui-hud` | Tam UI/HUD sistemi | Step 7 |
| Step 9 | `feature/step9-levels` | LevelManager, 10 level tasarımı | Step 8 |
| Step 10 | `feature/step10-audio` | Ses efektleri entegrasyonu | Step 9 |
| Step 11 | `feature/step11-polish` | VfxManager, mobil optimizasyon, görsel polish, final test | Step 10 |

---

## Teslim Kontrol Listesi (AGENT.md Bölüm 12)

Her step tamamlandığında şu çıktılar doğrulanmalı:

- [ ] **Proje tree:** hangi klasörde ne var (güncel)
- [ ] **Script listesi:** oluşturulan / değiştirilen script'ler
- [ ] **Kurulum adımları:** scene/prefab bağlamaları (inspector referanslar)
- [ ] **Oynanış test adımları:** Level 01'den 10'a nasıl test edilir
- [ ] **Bilinen riskler / TODO'lar:** MVP dışı kalan maddeler

---

## MVP Sonrası (Backlog)

- [ ] Rewarded reklam: +ink / undo / hint
- [ ] Interstitial reklam pacing
- [ ] Gate / button / moving obstacle mekanikleri
- [ ] Tutorial pop-up'lar (ilk 3 level)
- [ ] Müzik ekleme
- [ ] Haptic feedback (mobil)
- [ ] Analytics entegrasyonu
- [ ] Leaderboard / yıldız sistemi
- [ ] Daha fazla level (20+)
- [ ] Tema/skin sistemi
