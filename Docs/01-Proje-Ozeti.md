# Proje Ozeti

## Projenin Amaci

TriInkTrack, 2D fizikli bir bulmaca oyun prototipidir. Oyuncu, topun hedefe ulasmasi icin cizgiler cizer. Cizgiler farkli "murekkep" turlerine gore farkli fizik davranislari uretir:

- `Ice`: dusuk surtunme (kaygan)
- `Sticky`: yuksek surtunme (yavaslatan/tutan)
- `Bouncy`: yuksek sekiyor

Oyuncu ayni zamanda sinirli murekkep puaniyla oynar; bu da cizim kararlarini stratejik hale getirir.

## Hedeflenen Oynanis Dongusu

1. Oyun `Ready` veya `Playing` durumunda baslar.
2. Top otomatik olarak hareket eder.
3. Oyuncu ekranda cizgi olusturur.
4. Top cizgilere carparak yon degistirir.
5. Top hedefe girdiginde `Win` tetiklenir.
6. Basarisiz senaryoda `Fail` tetiklenmesi beklenir.

## Mimarinin Kisa Ozet

- Durum yonetimi: `GameManager`
- Cizim sistemi: `DrawSystem` + `InkLine` + `InkLinePool`
- Murekkep ekonomisi: `InkInventory`
- Top davranisi: `BallController`
- Hedef kontrolu: `TargetGoal`
- UI secim bari: `InkSelectionUI`

## Mevcut Scope

- Tek sahneli prototip (`GameScene`)
- Tek level yerlesimi
- 3 murekkep tipi aktif
- Cizgi omru ve fade animasyonu aktif
- Cizgi havuzu (pool) ile performans iyilestirmesi mevcut

Bu haliyle proje "core prototype" seviyesindedir: temel mekanik oynanabilir, fakat level sistemi, basarisizlik kurallari ve urunlestirme adimlari tamamlanmamistir.
