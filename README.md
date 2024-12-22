# KOÜ Yazılım Müh. Programlama Laboratuvarı III Dersi Projesi
Kocaeli Üniversitesi Mühendislik Fakültesi Yazılım Mühendisliği 24-25 Programlama Laboratuvarı III Projesi GitHub sayfası. Spacewar Oyunu.

# İçerik

- [Kullanılan Araçlar](https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar?tab=readme-ov-file#kullanılan-araçlar)
- [Amaç](https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar?tab=readme-ov-file#amaç)
- [Projeden Beklenenler](https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar?tab=readme-ov-file#projeden-beklenenler)
- [Karşılanan Beklentiler](https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar?tab=readme-ov-file#karşılanan-beklentiler)
- [İndirme ve Çalıştırma](https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar?tab=readme-ov-file#i%CC%87ndirme-ve-%C3%A7al%C4%B1%C5%9Ft%C4%B1rma)
- [Teşekkürler](https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar?tab=readme-ov-file#teşekkürler)
- [Oynanış Videosu](https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar?tab=readme-ov-file#oynan%C4%B1%C5%9F-videosu)

## Kullanılan Araçlar

<p align="center">
  <a href="https://learn.microsoft.com/tr-tr/dotnet/csharp/" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/ca28c779441053191ff11710fe24a9e6c23690d6/icons/csharp/csharp-original.svg" alt="csharp" width="40" height="40"/> </a>
  <a href="https://www.raylib.com/" target="_blank" rel="noreferrer"> <img src="https://github.com/raysan5/raylib/blob/master/logo/raylib_1024x1024.png?raw=true" alt="raylib" width="40" height="40"/> </a>
  <a href="https://www.jetbrains.com/rider/" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/ca28c779441053191ff11710fe24a9e6c23690d6/icons/rider/rider-original.svg" alt="rider" width="40" height="40"/> </a>
  <a href="https://www.adobe.com/tr/products/photoshop.html" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/photoshop/photoshop-original.svg" alt="photoshop" width="40" height="40"/> </a>
</p>

- Oyunun yapımında sadece C# dili kullanılmıtır.
- Geliştirme sürecinde JetBrains'in Rider yazılımı kullanılmıştır.
- Konsol uygulaması olarak geliştirildi.
- MacOS işletim sisteminde test edildi.

| Kullanılan Araç | Tavsiye Linkler |
|:---:|:---:|
| C# | [Microsoft](https://learn.microsoft.com/tr-tr/collections/yz26f8y64n7k07) |
| C# | [Murat Yücedağ C# Eğitim Kampı](https://youtube.com/playlist?list=PLKnjBHu2xXNPmFMvGKVHA_ijjrgUyNIXr&si=gL6c-oeP9LUJCN2u) |
| Raylib | [Tüm Raylib Özellikleri](https://www.raylib.com/cheatsheet/cheatsheet.html) |
| Raylib-cs | [C# Raylib Paketi](https://github.com/ChrisDill/Raylib-cs) |
| Sınıf Diyagramı | [Plant Text](https://www.planttext.com/) |
| İllüstrasyonlar | [Itch.io](https://itch.io/) |
| Ses Efektleri | [Freesound](https://freesound.org/) |
| Ses Efektleri | [Pixabay](https://pixabay.com/sound-effects/) |
| Çarpışma Sistemi | [Çember ve Üçgen Kesişim Algoritması](https://www.phatcode.net/articles.php?id=459) |
| Çarpışma Sistemi | [Üçgen içinde Nokta Testi / Same Side Tekniği](https://blackpawn.com/texts/pointinpoly/default.html) |
| Animasyon | [Animations Guide](https://www.sandromaglione.com/articles/pixel-art-character-animations-guide) |


## Amaç

YZM209 dersi kapsamında, bugüne kadar öğrendiğimiz bilgiler ve C# dili kullanılarak, belirlenen özelliklere sahip bir Spacewar Oyunu geliştirmek hedeflenmektedir. Proje, OOP prensiplerini (kalıtım, kapsülleme, polimorfizm) kullanarak uzay savaşı temasında bir oyun oluşturmayı ve öğrencilerin yazılım geliştirme becerilerini pekiştirmesini amaçlamaktadır.

## Projeden Beklenenler

```json
{
  "Anahtar Özellikler": {
    "Başlat": "Oyunu başlatma özelliği (StartGame).",
    "Güncelle": "Oyun sırasında oyuncu ve düşman hareketlerini güncelleme özelliği (UpdateGame).",
    "Çarpışma Algılama": "Mermiler ve uzay gemilerinin çarpışmalarını algılama özelliği (CheckCollisions).",
    "Bitir": "Oyun sona erdiğinde skor ve mesaj gösterme özelliği (EndGame)."
  },
  "Sınıflar ve Özellikleri": {
    "Game": {
      "Özellikler": [
        "Oyuncunun uzay gemisi (Spaceship).",
        "Düşman uzay gemilerinin listesi (List<Enemy>).",
        "Oyun bitiş durumu (isGameOver)."
      ],
      "Metotlar": [
        "StartGame(): Oyunu başlatır.",
        "UpdateGame(): Oyun akışını her karede günceller.",
        "CheckCollisions(): Çarpışmaları kontrol eder.",
        "EndGame(): Oyun bitiş işlemlerini yönetir."
      ]
    },
    "Spaceship": {
      "Özellikler": [
        "Sağlık durumu (health).",
        "Hız (speed).",
        "Ateşlenen mermilerin listesi (bullets)."
      ],
      "Metotlar": [
        "Move(direction): Belirtilen yöne hareket eder.",
        "Shoot(): Mermi ateşler.",
        "TakeDamage(amount): Sağlık puanını günceller."
      ]
    },
    "Enemy": {
      "Özellikler": [
        "Sağlık durumu (health).",
        "Hız (speed).",
        "Hasar miktarı (damage)."
      ],
      "Soyut Metotlar": [
        "Move(): Hareket algoritmasını uygular.",
        "Attack(): Oyuncuya saldırır."
      ]
    },
    "Bullet": {
      "Özellikler": [
        "Hız (speed).",
        "Hasar (damage).",
        "Yön (direction)."
      ],
      "Metotlar": [
        "Move(): Merminin ekrandaki ilerlemesini sağlar.",
        "OnHit(): Çarpışma durumunda hasar uygular ve mermiyi yok eder."
      ]
    },
    "CollisionDetector": {
      "Metotlar": [
        "CheckCollision(): Oyuncu ve düşman çarpışmalarını kontrol eder.",
        "CheckBulletCollision(): Mermilerin çarpışma durumlarını kontrol eder."
      ]
    }
  },
  "OOP Prensipleri": {
    "Kalıtım": "Enemy sınıfı, farklı düşman türlerinin temel sınıfıdır.",
    "Polimorfizm": "Farklı düşman türleri aynı metotları farklı şekilde uygular.",
    "Kapsülleme": "Özellikler private olarak tutulur ve sadece metotlar aracılığıyla erişilir."
  },
  "Ek İşlevsellikler": {
    "Power-Ups": "Ekstra can, hız veya hasar kazandıran güçlendirme nesneleri.",
    "Level Sistemi": "Artan zorluk seviyeleri ile oyunun ilerleyişi.",
    "Puan Sistemi": "Farklı düşman türlerinden kazanılan puanlar.",
    "Engel Yapıları": "Ekranda yer alan ve çarpışma durumunda hasar veren engeller."
  },
  "Görsel ve Teknik Gereklilikler": {
    "GUI": "Grafiksel kullanıcı arayüzü oluşturulması.",
    "Dil": "C# programlama dili kullanılması.",
    "Raporlama": "Proje kodlarının ve açıklamalarının raporlanması."
  }
}

```

## Karşılanan Beklentiler

| Beklenti | Durum | Detay |
|:---:|:---:|:---:|
| Anahtar Özellikler | ✅ | - |
| Sınıflar ve Özellikleri | ✅ | Tüm sınıf özellikleri sağlanmıştır. Ek olarak ihtiyaçlar dahilinde <br>ekstra sınıflar da oluşturulmuştur. Bullet sınıfı da <br>soyut sınıf haline getirilip çeşitlilik sağlanmıştır. |
| OOP Prensipleri | ✅ | - |
| Ek İşlevsellikler | ⚠️ | Engel Yapıları hariç diğer tüm ek özellikler sağlanmıştır. <br>Engel yapıları ekranda oluşan karışıklık nedeniyle tercih edilmemiştir. <br>Tasarlanan görseller "kaynak" klasöründedir. |
| Görsel ve Teknik Gereklilikler | ✅ | Ekstra olarak kullanıcıya oyunun dili, müzik sesi ve efekt sesi <br>gibi özellikleri değiştirme imkanı sağlanmıştır. |

## İndirme ve Çalıştırma

[Bu bağlantıyı](https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar/archive/refs/tags/game.zip) kullanarak kaynak dosyaları indirebilir ve projemizi yerel olarak çalıştırabilirsiniz. Git üzerinden indirmek isterseniz:
```
git clone https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar.git
```

### Sadece Oynamak İsterseniz

| Platform | İndirme Linki |
|:---:|:---:|
| win-x64 | [Tıkla](https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar/releases/download/game/spacewar_win-x64.zip) |
| osx-x64 | [Tıkla](https://github.com/metehansenyer/KOU-YZM209-CSGameProject-Spacewar/releases/download/game/spacewar_osx-x64.zip) |

> [!IMPORTANT]  
> MacOS kullanıcıları oyunu açmak için dosyayı çıkardıkları dizini terminalde açıp <br>.<b>/KOU-YZM209-CSGameProject-Spacewar komutunu kullanmalılar.</b> <br>Normal açıldığında kaplamalar yüklenmiyor.

> [!NOTE]  
> Sorun üzerine çok uğraşmaya vaktim olmadığı için çözemedim. <br>Çözüm önerilerine açığım. *.app haline getirmek işe yaramadı.

## Teşekkürler

Zor şartlar altındaki proje sunumumdaki yardımlarından dolayı Kocaeli Üniversitesi Yazılım Mühendisliği Bölümü'den Araştırma Görevlisi Melike Bektaş Kösesoy ve Araştırma Görevlisi Şevval Şolpan hocama teşekkürlerimi arz ederim.

## Oynanış Videosu

https://github.com/user-attachments/assets/6a81e164-f780-4484-bbac-dca6b7bd7c39
