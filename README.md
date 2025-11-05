# PROJECTAVATAR

_Unleash Creativity, Elevate Experiences Instantly_

[![last commit](https://img.shields.io/github/last-commit/FzLse/ProjectAvatar?label=last%20commit)](https://github.com/FzLse/ProjectAvatar/commits)
![C#](https://img.shields.io/badge/c%23-75.7%25-blue)
![languages](https://img.shields.io/github/languages/count/FzLse/ProjectAvatar?label=languages)

**Built with the tools and technologies:**  
![JSON](https://img.shields.io/badge/-JSON-black?logo=json&logoColor=white)
![Unity](https://img.shields.io/badge/-Unity-black?logo=unity&logoColor=white)

---

## Table of Contents

- [Overview](#overview)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Usage](#usage)
  - [Testing](#testing)

---

## Overview

ProjectAvatar adalah proyek game & service kecil yang menggabungkan **Unity (client)** dan **.NET** untuk fitur seperti leaderboard dan penyimpanan progres. Repositori ini berfungsi sebagai titik awal yang ringkas untuk menjalankan service/API lokal dan (opsional) menghubungkan game client.

> Catatan: Sesuaikan deskripsi singkat ini dengan fokus repositori kamu (hanya server/.NET saja atau juga Unity project).

---

## Getting Started

### Prerequisites

Proyek ini memerlukan dependensi berikut:

- **Programming Language**: CSharp (.NET SDK 8.0 atau lebih baru)
- **Package Manager**: NuGet
- (Opsional) **Unity Editor**: 2022.3 LTS atau yang kamu gunakan pada client

---

### Installation

Bangun _ProjectAvatar_ dari source dan pasang dependensinya.

1. **Clone the repository**

   ```bash
   git clone https://github.com/FzLse/ProjectAvatar
   ```

2. **Navigate to the project directory**

   ```bash
   cd ProjectAvatar
   ```

3. **Install the dependencies** (via NuGet / dotnet)

   ```bash
   dotnet restore
   ```

> Bila server berada di sub-folder (mis. `Server/`), masuk dulu ke folder tersebut sebelum menjalankan perintah `dotnet`.

---

### Usage

Jalankan proyek dengan:

```bash
dotnet run
```

Secara default server akan berjalan di `http://localhost:5000` atau `https://localhost:5001` (tergantung pengaturan Kestrel/HTTPS). Sesuaikan `BASE_URL` di Unity client jika diperlukan.

---

### Testing

ProjectAvatar menggunakan _xUnit_ sebagai kerangka uji. Jalankan test suite dengan:

```bash
dotnet test
```

---

## License

MIT (atau lisensi yang kamu pilih).

## Contributing

Kontribusi terbuka! Silakan ajukan _issue_ atau _pull request_ untuk perbaikan/penambahan fitur.

## Acknowledgements

- Unity
- .NET / ASP.NET Core
- Community packages yang relevan
