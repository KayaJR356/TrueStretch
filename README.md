# TrueStretch 3.0

TrueStretch, Windows'ta desteklenen ekran modlarını güvenli biçimde uygulayan, özel çözünürlük profilleri saklayan ve rekabetçi oyunlar için web kaynaklı çözünürlük önerileri çıkaran açık kaynaklı bir WinForms aracıdır.

> [!WARNING]
> Ekran modu değişiklikleri geçici görüntü kaybına yol açabilir. TrueStretch yeni modu uygulamadan önce sürücüyle test eder ve kullanıcı 15 saniye içinde onaylamazsa önceki moda döner. Uygulama imzasızdır; yalnızca bu depodan kendiniz derlediğiniz veya Releases bölümünden hash'i doğrulanmış çıktıyı kullanın.

## Özellikler

- Windows sürücüsünün bildirdiği çözünürlük ve yenileme hızlarını listeler.
- Seçilen modu uygulamadan önce `CDS_TEST` ile doğrular.
- 15 saniyelik güvenli onay ve otomatik geri alma sunar.
- İsteğe bağlı olarak TrueStretch profili aktifken monitör aygıtını devre dışı tutar; normal moda dönüşte yeniden etkinleştirir.
- Düşük çözünürlükleri `DMDFO_STRETCH` ile tam ekrana yayarak sürücü desteklediğinde sağ ve sol siyah çubukları kaldırır.
- Genişlik, yükseklik ve yenileme hızından özel profiller oluşturur.
- Profilleri `%LocalAppData%\TrueStretch\profiles.xml` altında kullanıcı bazında saklar.
- Valorant, Counter-Strike 2, Fortnite, Apex Legends, PUBG, Rainbow Six Siege, Overwatch 2 ve The Finals için web önerileri arar.
- Web sonuçlarında geçen çözünürlükleri sıklıklarına göre sıralar ve kaynak bağlantısını korur.
- İnternet erişimi yoksa başlangıç önerilerine geri döner.
- Tek dosyalık, kurulum gerektirmeyen 64-bit Windows uygulaması üretir.

## Ekranlar

### Ekran Modları

Sürücünün desteklediği modları listeler. Seçilen mod test edilir, uygulanır ve geri sayımlı pencerede onay bekler.

### Özel Profiller

Çözünürlük ve yenileme hızı kombinasyonlarını kaydeder. Profil, sürücü tarafından desteklenmiyorsa TrueStretch güvenli biçimde reddeder.

### Oyun Önerileri

Seçilen oyun için Bing RSS aramasını kullanır. Sonuç başlığı ve açıklamalarındaki `genişlik × yükseklik` değerlerini çıkarır, tekrar sayısına göre sıralar ve orijinal kaynağı açmanıza olanak verir. Bu öneriler topluluk verisidir; donanım uyumluluğu garantisi değildir.

## Sistem gereksinimleri

- Windows 10 veya Windows 11, 64-bit
- .NET Framework 4.x
- Mod uygulamak için yönetici yetkisi
- Web önerileri için internet bağlantısı

## Derleme

Depoyu klonlayın ve PowerShell'de:

```powershell
.\build.ps1
```

Derleme, Windows ile gelen 64-bit .NET Framework C# derleyicisini kullanır. Çıktı üst dizindeki `outputs\TrueStretch.exe` konumuna yazılır.

GitHub Actions, her push ve pull request için Windows üzerinde aynı derlemeyi otomatik doğrular ve EXE'yi artifact olarak sunar.

## Teknik yaklaşım

- Arayüz: Windows Forms
- Ekran modu keşfi: `EnumDisplaySettings`
- Güvenli ön kontrol: `ChangeDisplaySettingsEx(..., CDS_TEST, ...)`
- Uygulama: `ChangeDisplaySettingsEx(..., CDS_UPDATEREGISTRY, ...)`
- Profil biçimi: Basit XML
- Web araması: HTTPS üzerinden Bing RSS
- Bağımlılıklar: Yalnızca .NET Framework sistem kütüphaneleri

Daha ayrıntılı akış için [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) dosyasına bakın.

## Bilinen sınırlamalar

- TrueStretch, ekran kartı sürücüsüne yeni EDID veya özel zamanlama kaydı zorla yazmaz.
- Onay süresi dolarsa veya uygulama kapanırsa monitör aygıtı güvenlik amacıyla yeniden etkinleştirilir.
- Bir profil kaydedilebilir olsa da sürücü tarafından desteklenmiyorsa uygulanamaz.
- NVIDIA, AMD ve Intel kontrol panellerinde daha önce oluşturulan özel modlar Windows tarafından görünüyorsa listelenebilir.
- Çoklu monitör desteği şu anda birincil ekranla sınırlıdır.
- Web sonucu sıklığı, önerinin teknik olarak en iyi seçenek olduğunu tek başına kanıtlamaz.
- Bazı oyunların en-boy oranı ve tam ekran davranışı oyun motoruna göre değişir.

## Güvenlik ve gizlilik

Uygulama telemetri toplamaz ve kullanıcı hesabı istemez. Web önerisi çalıştırıldığında seçilen oyun adı Bing aramasına gönderilir. Profil verileri yalnızca yerel kullanıcı klasöründe tutulur. Güvenlik bildirimi için [`SECURITY.md`](SECURITY.md) dosyasını kullanın.

## Katkı

Hata raporları ve geliştirmeler memnuniyetle karşılanır. Değişiklik göndermeden önce [`CONTRIBUTING.md`](CONTRIBUTING.md) dosyasını okuyun.

## Lisans

MIT — ayrıntılar için [`LICENSE`](LICENSE).
