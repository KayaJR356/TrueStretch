# Mimari

## Genel akış

1. Uygulama açılırken mevcut birincil ekran modu kaydedilir.
2. Windows'un bildirdiği modlar `EnumDisplaySettings` ile toplanır ve tekilleştirilir.
3. Kullanıcı bir mod seçtiğinde `CDS_TEST` çağrısı sürücünün modu kabul edip etmediğini denetler.
4. Başarılı sonuçta mod uygulanır ve 15 saniyelik onay penceresi açılır.
5. Kullanıcı onay vermezse işlem öncesindeki mod yeniden uygulanır.

## Bileşenler

- `MainForm`: Sekmeler, profiller ve öneri listesinin koordinasyonu.
- `DisplayApi`: Win32 ekran modu çağrıları ve hata kodlarının kullanıcı mesajlarına çevrilmesi.
- `TimedConfirm`: Değişiklik sonrası geri sayım ve otomatik geri alma.
- Web öneri akışı: RSS indirme, XML ayrıştırma, çözünürlük desenlerini çıkarma ve sıralama.

## Veri saklama

Profiller `%LocalAppData%\TrueStretch\profiles.xml` dosyasına yazılır. Şema:

```xml
<profiles>
  <mode width="1568" height="1080" hz="60" />
</profiles>
```

## Güven sınırları

- İnternetten gelen başlık ve açıklamalar düz metne dönüştürülür.
- Web içeriği uygulama içinde HTML olarak çalıştırılmaz.
- Yalnızca kullanıcı açıkça “Kaynağı aç” dediğinde varsayılan tarayıcı başlatılır.
- Ekran modu sürücü ön kontrolü başarısızsa uygulanmaz.
- Uygulama desteklenmeyen sürücü kayıtlarını veya EDID verisini değiştirmez.

## Gelecek geliştirmeler

- Birden fazla monitör seçimi
- Üretici API'leriyle isteğe bağlı özel zamanlama modülleri
- Öneri kaynaklarında alan adı filtreleme ve güven puanı
- Kod imzalı sürümler
- Yerelleştirme kaynak dosyaları

