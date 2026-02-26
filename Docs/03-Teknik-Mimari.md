# Teknik Mimari

## 1) Oyun Durum Yonetimi

`GameManager` state machine:

- `Ready`
- `Playing`
- `Win`
- `Fail`

Ana olay: `OnGameStateChanged`.

Diger sistemler bu event'e subscribe olarak davranis degistiriyor:

- `BallController`: `Win/Fail` durumunda topu durdurur, `Ready` olunca resetler.
- `DrawSystem`: hangi durumda cizime izin verilecegini belirler.
- `InkInventory`: `Ready` olunca murekkebi yeniler.

## 2) Cizim Pipeline

`DrawSystem` gorevleri:

- Mouse + touch input yakalama (Input System)
- UI ustunde cizimi engelleme
- Oyun alani disinda cizimi engelleme
- Cizgi limiti ve "en eskiyi sil" stratejisi
- Murekkep kontrolu (`HasInk` / `ConsumeInk`)
- Cizgi omru/fade baslatma

`InkLine` gorevleri:

- `LineRenderer` gorseli
- `EdgeCollider2D` fizik kenari
- Nokta ekleme ve kilitleme
- Murekkep tipine gore renk + fizik materiali uygulama

`InkLineLifetime`:

- Omur suresi sonra fade
- Sure bitince pool'a iade

`InkLinePool`:

- Cizgi instantiate maliyetini azaltir
- Prewarm + tekrar kullanim

## 3) Fizik ve Murekkep Tipleri

`InkType`:

- `Ice`
- `Sticky`
- `Bouncy`

`InkLine`, tipe gore `PhysicsMaterial2D` atar:

- Ice: dusuk surtunme, dusuk sekme
- Sticky: yuksek surtunme
- Bouncy: yuksek sekme

## 4) UI Katmani

`InkSelectionUI`:

- 3 buton ile murekkep secimi
- secili buton vurgusu (scale + alpha)
- murekkep meter guncellemesi (`Image.fillAmount`)
- `InkInventory.OnInkChanged` event'i ile senkron

## 5) Konfigurasyon

`GameConfig` ScriptableObject degerleri:

- top hizi
- min nokta mesafesi
- cizgi nokta limiti
- aktif cizgi limiti
- toplam murekkep puani
- cizgi omur/fade sureleri

Bu degerler runtime'da ilgili sistemlere override ediliyor.

## 6) Editor Otomasyonlari

- `Step2SetupTool`: top/hedef/duvar prefab + test level kurulumu
- `Step3SetupTool`: DrawSystem + InkLine prefab kurulumu
- `Step5SetupTool`: fizik materyalleri + UI + InkInventory baglantilari

Bu araclar hizli yeniden kurulum icin kritik.
