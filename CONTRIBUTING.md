# TrueStretch'e Katkı Rehberi

TrueStretch'e katkıda bulunduğunuz için teşekkürler. Bu rehber, değişikliklerin güvenli, incelenebilir ve proje kapsamıyla uyumlu kalmasını sağlar.

## Başlamadan önce

- [Davranış Kuralları](CODE_OF_CONDUCT.md) belgesini okuyun.
- Hata veya öneriniz için mevcut [issue'ları](https://github.com/KayaJR356/TrueStretch/issues) arayın.
- Güvenlik açıklarını herkese açık issue olarak bildirmeyin; [SECURITY.md](SECURITY.md) belgesini izleyin.
- Büyük değişikliklerden önce kapsamı netleştiren bir issue açın.

## Geliştirme ortamı

- Windows 10 veya Windows 11 (64-bit)
- .NET Framework 4.x
- PowerShell
- Git

```powershell
git clone https://github.com/YOUR_USERNAME/TrueStretch.git
cd TrueStretch
.\build.ps1
```

## Önerilen iş akışı

1. Depoyu fork edin.
2. Açıklayıcı bir dal oluşturun: `fix/safe-mode-message` veya `docs/install-guide`.
3. Değişikliğinizi küçük ve tek amaçlı tutun.
4. Derlemeyi ve ilgili kullanıcı akışlarını doğrulayın.
5. Anlamlı bir commit mesajı yazın.
6. Pull request şablonunu eksiksiz doldurun.

## Kalite ve güvenlik kontrolü

- `.\build.ps1` hatasız tamamlanmalıdır.
- Ekran modu kodu değiştiyse desteklenen ve desteklenmeyen en az bir mod doğrulanmalıdır.
- Otomatik geri alma ve başlangıç moduna dönüş davranışı korunmalıdır.
- Win32 yapı alanları 64-bit Windows'ta doğrulanmalıdır.
- Ağdan gelen içerik çalıştırılmamalı veya ham HTML olarak gösterilmemelidir.
- Yeni bağımlılıklar yalnızca açık gerekçe ve bakım değerlendirmesiyle önerilmelidir.
- Kullanıcı verisi, telemetri veya ağ davranışı ekleniyorsa PR'da açıkça belirtilmelidir.

> [!WARNING]
> Ekran modu ve monitör aygıtı işlemleri görüntü kaybına neden olabilir. Testten önce çalışmalarınızı kaydedin ve geri dönüş yolunu doğrulayın.

## Pull request beklentileri

PR açıklamasında şunları belirtin:

- Sorun ve çözüm
- Kullanıcı etkisi
- Yapılan doğrulamalar
- Varsa ekran görüntüsü
- Güvenlik, gizlilik veya geriye dönük uyumluluk etkisi
- İlgili issue

Bakımcılar kapsamın küçültülmesini, ek test veya dokümantasyon isteyebilir.

## Dokümantasyon

Kullanıcı davranışını değiştiren katkılar README, CHANGELOG veya mimari belgesini de güncellemelidir. Doğrulanmamış bilgi eklemeyin; gerekirse `TODO` kullanın.

## Lisans

Katkı göndererek katkınızın projenin [MIT Lisansı](LICENSE) altında yayımlanmasını kabul etmiş olursunuz.
