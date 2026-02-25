# Tri‑Ink Track (Üç Mürekkep Parkuru)

Bu dosya, **Codex / Claude Code / GeminiPro** gibi bir “coding agent”e, GDD’ye göre **Unity 2D** üzerinde **MVP’yi** uçtan uca çıkarttırmak için hazırlanmış yönlendirici çalışma talimatıdır.

---

## 0) Misyon

**Hedef:** Mobil (iOS/Android) için tek parmakla çizilen 3 farklı “mürekkep” yüzeyi ile yön verilen, top‑down 2D fizik tabanlı puzzle oyununun **MVP** sürümünü Unity’de çalışır hale getir.

**MVP çıktısı:**
- Oynanabilir 10 level
- 3 ink (Ice/Sticky/Bouncy) belirgin his farkı
- Çizgiler zamanla siliniyor (fade + collider disable)
- Win/Fail + Retry
- Mobilde stabil performans (pooling + segment limit)

---

## 1) Ürün Özeti (Agent Context)

### Oyun fikri
- Top‑down 2D ortamda bir **bilye otomatik hareket eder**.
- Oyuncu 3 ink’ten biriyle **kısa çizgiler** çizer.
- Bilye çizgilere temas edince fizik etkisi uygulanır.
- Çizimler **5–10 sn** sonra kaybolur.

### Üç ink ve beklenen fizik hissi
- **Ice:** düşük sürtünme, hız koru/kaydır.
- **Sticky:** yüksek sürtünme, fren/durdur.
- **Bouncy:** yüksek esneklik, sekme ile yön kır.

---

## 2) Teknoloji ve Proje Varsayımları

- Engine: **Unity 2D (Built‑in veya URP)**
- Fizik: **Unity Physics2D**
- Kod: **C#**
- Kontrol: **tek parmak çizim** (touch + mouse simülasyonu)

### Performans hedefi (MVP)
- Çizim sisteminde runtime allocation minimize edilecek.
- Çizgi üretimi + yok oluşu **pooling** ile yönetilecek.

---

## 3) Sahne / Prefab / Klasör Organizasyonu

### Tek sahne MVP
- `GameScene`
  - `GameManager`
  - `LevelRoot` (aktif level içeriği)
  - `Ball`
  - `Target`
  - `DrawSystem`
  - `UI Canvas`

### Prefab: InkLine
- `LineRenderer` (görsel)
- `EdgeCollider2D` (fizik)
- `InkLineLifetime` (fade + pool)
- `InkType` (enum)
- `PhysicsMaterial2D` (ink’e göre)

### Önerilen klasör yapısı
- `Assets/_Game/Art/…`
- `Assets/_Game/Audio/…`
- `Assets/_Game/PhysicsMaterials/`
- `Assets/_Game/Prefabs/`
- `Assets/_Game/Scenes/`
- `Assets/_Game/Scripts/`

---

## 4) Core Gameplay Kuralları

### Game State
- `Ready → Playing → Win/Fail → Retry/Next`

### Win/Fail
- **Win:** Ball `Target` trigger’a girer.
- **Fail:** hazard teması veya out‑of‑bounds.

---

## 5) Sistem Tasarımı (Kod Modülleri)

Aşağıdaki sınıflar MVP için “minimum ama temiz” bir mimaridir. Agent, gerekirse ek yardımcı sınıflar ekleyebilir; ancak bu listeyi referans almalıdır.

### Script listesi
- `GameManager` (state, win/fail)
- `LevelManager` (load/reset)
- `BallController` (auto move + reset)
- `DrawSystem` (input → points → line)
- `InkLine` (renderer + collider sync)
- `InkLineLifetime` (timer, fade, collider disable, pool return)
- `InkLinePool` (pool per ink)
- `InkInventory` (ink puanı tüketimi)
- `UIHudController` (ink select, ink bar, retry/next)
- `AudioManager` (SFX)
- `VfxManager` (opsiyonel MVP)

---

## 6) Kritik Mekanikler — Uygulama Spesifikasyonu

### 6.1 Ball auto‑move
**Amaç:** Bilye sürekli hareket halinde kalsın, hissi stabil olsun.

- `Rigidbody2D.gravityScale = 0`
- MVP yaklaşımı: **hız normalizasyonu**
  - `FixedUpdate` içinde `rb.velocity = rb.velocity.normalized * targetSpeed` yaklaşımı
  - Çok düşük hızlarda fallback yön (`moveDir`) ile min hız ver

**Konfig:**
- `targetSpeed`: 4–7 (world unit/sn)
- `linearDrag`: 0.1–0.4

### 6.2 DrawSystem (input → line)
**Amaç:** Parmak sürüklenirken pürüzsüz çizgi üret.

- Screen→World dönüşümü
- Nokta örnekleme: `minPointDist = 0.12–0.2 world`
- LineRenderer güncelle
- EdgeCollider2D üret (local space)
- Kısıtlar:
  - toplam ink puanı (ör. 100)
  - max aktif çizgi (ör. 30)
  - max point sayısı / max segment

**Input Notu:**
- Mobilde touch; editörde mouse ile simüle.
- UI üstüne çizim engellenmeli (EventSystem raycast check).

### 6.3 InkLine collider sync
- `LineRenderer.SetPositions(points)`
- `EdgeCollider2D.points = ConvertToLocal(points)`

### 6.4 Lifetime + fade + pooling
- Her inkline spawn olduğunda `spawnTime` kaydet.
- `lifeSeconds` dolunca:
  - 0.5 sn fade
  - collider disable
  - pool’a iade

**Pool:**
- Her ink tipi için ayrı pool veya tek pool + type switch.

### 6.5 PhysicsMaterial2D ayarları
MVP için 3 adet `PhysicsMaterial2D` oluştur:
- **Ice:** friction 0–0.05, bounciness 0–0.1
- **Sticky:** friction 0.8–1, bounciness 0
- **Bouncy:** friction 0–0.2, bounciness 0.8–1

Agent, sahneye göre “hissedilen sonucu” iyileştirmek için küçük iterasyonlar yapabilir, ama üç ink **bariz farklı** olmalı.

---

## 7) UI / UX (MVP)

### HUD bileşenleri
- 3 ink seçim butonu (alt bar)
- Ink bar (remaining ink)
- Retry
- (Opsiyonel) Pause

### UX kuralları
- Seçili ink net görünür.
- Çizgi bitince (pointer up) çizgi “kilitlenir” ve ömrü geri sayar.

---

## 8) Level Sistemi (MVP)

### Format
MVP’de level’ları hızlı çıkarmak için iki seçenekten birini seç:

**Seçenek A (Hızlı):**
- Level’lar prefabs: `Level_01.prefab … Level_10.prefab`
- `LevelManager` seçili prefabı `LevelRoot` altına instantiate eder.

**Seçenek B (Bir tık daha sistematik):**
- Basit `ScriptableObject LevelDefinition` + prefab referansı.

### Örnek 10 level
- Agent, GDD’deki 10 ASCII şemayı Unity’de yaklaşık layout ile kuracak.
- Her level “tek fikir” öğretmeli.

---

## 9) Kabul Kriterleri (Done Checklist)

Agent, her maddeyi gözle doğrulayacak şekilde teslim etmeli:

- [ ] Ball, level başlar başlamaz otomatik hareket ediyor.
- [ ] Çizim, tek parmakla pürüzsüz; minPointDist ile segment patlamıyor.
- [ ] 3 ink birbirinden belirgin: Ice kaydırıyor, Sticky frenliyor, Bouncy sektiriyor.
- [ ] Çizgiler 5–10 sn sonra fade olup siliniyor; collider kapanıyor.
- [ ] Pooling çalışıyor (GC spike yok / minimal).
- [ ] Win: Target’a girince state Win, UI Next açılıyor.
- [ ] Fail: hazard/out‑of‑bounds state Fail, Retry çalışıyor.
- [ ] 10 level oynanabilir.

---

## 10) Agent Çalışma Planı (İteratif Teslim)

Agent, işi küçük PR/commit adımlarına bölmeli. Her adım çalışır durumda olmalı.

1) **Bootstrap**
- Proje ayarları, GameScene, temel GameManager state.

2) **Ball + Target**
- Ball prefab + auto move
- Target trigger + win

3) **DrawSystem v1**
- Input to points + LineRenderer
- minPointDist

4) **EdgeCollider2D**
- Collider güncelleme + local transform

5) **Ink Types**
- 3 PhysicsMaterial2D
- UI ile ink seçimi

6) **Lifetime + Pooling**
- fade, collider disable, pool
- max active line limit

7) **Fail Conditions**
- hazard + out‑of‑bounds

8) **10 Level**
- Level prefabs + LevelManager

9) **Polish (MVP sınırında)**
- Basit SFX
- Basit VFX (ops.)
- Mobil input test

---

## 11) Kod Standartları (Agent Kuralları)

- Public API’leri minimize et, inspector için `[SerializeField] private …` tercih et.
- `Update`/`FixedUpdate` içinde allocation yapma.
- Pooling: instantiate/destroy döngüsünden kaçın.
- Çizgi noktaları için `List<Vector3>` yeniden kullan (clear + reuse).
- Parametreleri `ScriptableObject` veya `Serializable` config struct ile tek yerde topla.
- Debug için `#if UNITY_EDITOR` loglarını kontrollü kullan.

---

## 12) Teslim Formatı (Agent Output)

Agent tesliminde şunları yazmalı:

1) **Proje tree** (hangi klasörde ne var)
2) Oluşturulan / değiştirilen script’lerin listesi
3) Kurulum adımları (scene/prefab bağlamaları)
4) Oynanış test adımları (Level 01’den 10’a)
5) Bilinen riskler / TODO’lar (MVP dışı)

---

## 13) Opsiyonel Geliştirmeler (MVP sonrası)

- Rewarded: +ink / undo / hint
- Interstitial pacing
- Gate/button/moving obstacle
- Tutorial pop‑up’lar

---

## 14) Hızlı Notlar

- Çizgi hissi kötüleşirse ilk ayar: `minPointDist` artır + max point clamp.
- Bouncy çok agresifse: bounciness düşür veya çarpışma sonrası maxSpeed clamp.
- Okunabilirlik: duvar koyu, zemin açık; target pulse.

