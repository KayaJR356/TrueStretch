# Değişiklik Geçmişi

Bu projedeki önemli değişiklikler bu dosyada belgelenir. Biçim [Keep a Changelog](https://keepachangelog.com/tr/1.1.0/) yaklaşımını izler ve sürümler [Semantic Versioning](https://semver.org/) ile numaralandırılır.

## [Unreleased]

### Documentation

- README, katkı, güvenlik, destek ve topluluk belgeleri profesyonel açık kaynak standartlarına göre düzenlendi.
- Issue ve pull request şablonları eklendi.

## [3.0.0] - 2026-07-20

### Added

- Sol navigasyonlu modern koyu arayüz ve tutarlı işlem alanları
- Güvenli ekran modu listeleme ve uygulama
- 15 saniyelik otomatik geri alma
- Yerel özel çözünürlük profilleri
- Sekiz oyun için web kaynaklı öneri araması
- Çevrimdışı başlangıç önerileri
- Kaynak bağlantısı ve en-boy oranı gösterimi
- Yönetici manifesti, uygulama simgesi ve otomatik Windows derlemesi
- `DMDFO_STRETCH` tam ekran ölçekleme isteği
- TrueStretch oturumunda monitör aygıtını devre dışı tutma seçeneği

### Changed

- Ekran Modları, Özel Profiller ve Oyun Önerileri ayrı çalışma alanlarına taşındı.
- İstenen yenileme hızı yoksa aynı çözünürlükte en yakın sürücü modu seçiliyor.
- Güvenli geri alma bilgisi ana arayüzde sürekli görünür hale getirildi.

### Fixed

- Menü/içerik eşleşmesi, beyaz sekme çerçevesi ve kesilen sayı alanları
- Çözünürlük desteği ile stretch ölçekleme kontrolünün karışmasından kaynaklanan yanlış `BADMODE` sonucu
- Mod değişiminden sonra monitör ölçekleme sıfırlaması ve normal moda dönüş davranışı

[Unreleased]: https://github.com/KayaJR356/TrueStretch/compare/v3.0.0...HEAD
[3.0.0]: https://github.com/KayaJR356/TrueStretch/releases/tag/v3.0.0
