# Güvenlik Politikası

## Desteklenen sürümler

| Sürüm | Destek |
| --- | --- |
| En son ana sürüm | ✅ |
| Önceki sürümler | ❌ |

Yalnızca en güncel ana sürüm için güvenlik düzeltmesi planlanır.

## Güvenlik açığı bildirme

Bir açığı herkese açık issue, discussion veya pull request içinde paylaşmayın.

1. Repository'de **Security → Report a vulnerability** seçeneğini kullanın.
2. Özel bildirim etkin değilse, teknik ayrıntı vermeden özel iletişim kanalı isteyen bir issue açın.
3. Etkilenen sürümü, yeniden üretim adımlarını, olası etkiyi ve varsa güvenli çözüm önerisini ekleyin.

Bu topluluk projesinde garanti edilen bir yanıt veya çözüm süresi yoktur. Doğrulanmış ve yüksek etkili bildirimler öncelikli ele alınır.

> [!IMPORTANT]
> Bakımcı güvenli bir düzeltme yayımlamadan önce açığı kamuya açıklamayın.

## Çalıştırma güvenliği

- Çalıştırılabilir dosya henüz kod imzalı değildir.
- Yalnızca bu depodan derlediğiniz veya güvenilir Release çıktısını kullanın.
- Yayımlanmışsa SHA-256 değerini doğrulayın.
- Ekran modu değişikliklerinden önce açık çalışmalarınızı kaydedin.
- Uygulama yönetici yetkisi ister ve monitör aygıt durumunu değiştirebilir.
- Yeni mod önce sürücünün `CDS_TEST` çağrısıyla sınanır.
- Onaylanmayan değişiklik 15 saniye içinde geri alınır.

## Kapsam

Güvenlik bildirimleri özellikle şu alanları kapsar:

- Yetki yükseltme veya komut çalıştırma
- Güvenilmeyen web içeriğinin işlenmesi
- Güvenli geri alma mekanizmasının atlatılması
- Yerel profil verisinin izinsiz okunması veya değiştirilmesi
- Release dosyalarının bütünlüğü

Genel hatalar için [bug report](https://github.com/KayaJR356/TrueStretch/issues/new?template=bug_report.md) kullanın.
