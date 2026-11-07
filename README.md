# Control Mastología V2 — Sistema de Gestión Clínica Especializada

<p align="center">
  <span style="font-size: 64px;">🏥</span>
  <br />
  <strong>Software clínico para registro de pacientes, anamnesis, historias médicas y diagnósticos mastológicos en consultorio.</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/WPF-Windows%20Desktop-0078D7?logo=windows&logoColor=white" alt="WPF" />
  <img src="https://img.shields.io/badge/Base%20de%20Datos-SQLite%20(Local)-003B57?logo=sqlite&logoColor=white" alt="SQLite" />
  <img src="https://img.shields.io/badge/Sector-Salud%20%26%20MedTech-E91E63" alt="Salud" />
  <img src="https://img.shields.io/badge/Estado-Producci%C3%B3n%20Local-success" alt="Estado" />
</p>

---

## 🩺 Contexto e Historia del Proyecto

> **Nota del autor:** Este fue mi **primer proyecto de software real**, desarrollado para resolver una necesidad clínica concreta: digitalizar y agilizar la consulta diaria de una especialista en mastología y patología mamaria. 
>
> La doctora requería una solución **100% offline, ágil y con máxima privacidad para sus pacientes en el consultorio**, sin depender de conexión a internet ni costos de servidores externos. Por ello, el núcleo que se utilizó en producción fue la aplicación de escritorio en **WPF (.NET) con almacenamiento local en SQLite** y respaldos automáticos. *(El proyecto incluye además un prototipo inicial de Web API que comencé a explorar con fines de sincronización remota).*

---

## 🚀 Funcionalidades Principales

### 📋 1. Ficha de Identificación y Registro Clínico
* **Búsqueda instantánea por Cédula:** Carga automática de los datos clínicos si el paciente ya ha asistido a consulta previa.
* **Datos demográficos:** Nombres, fecha de nacimiento, cálculo de edad, sexo y datos de contacto.
* **Anamnesis completa:**
  * Descripción detallada del motivo de consulta y enfermedad actual.
  * Registro de antecedentes personales y patológicos.
  * Registro de antecedentes familiares (especial énfasis en factores de riesgo oncológico/mamario).

### 🔬 2. Módulo de Diagnóstico y Consulta Especializada
* Ventana dedicada para la evaluación médica mastológica (`DiagnosticWindow`).
* Registro de hallazgos clínicos, evolución y tratamientos indicados.
* Generación de informes médicos y réipes clínicos estructurados en formato imprimible/PDF (utilizando QuestPDF).

### 🔒 3. Seguridad de Datos y Respaldos Clínicos
* Acceso protegido por contraseña para resguardo de la confidencialidad médica.
* **Sistema de Respaldos Automáticos:** Exportación de copias de seguridad de la base de datos con marca de tiempo en una carpeta segura para evitar pérdida de información ante fallos del equipo.

---

## 🛠️ Stack Tecnológico

| Componente | Tecnología |
| :--- | :--- |
| **Lenguaje** | C# (.NET) |
| **Frontend Desktop** | Windows Presentation Foundation (WPF), XAML |
| **Motor de Base de Datos** | SQLite (Microsoft.Data.Sqlite) / ADO.NET |
| **Generación de Informes** | QuestPDF |
| **Backend Exploratorio** | ASP.NET Core Web API *(prototipo)* |

---

## 📁 Estructura del Proyecto

```text
ControlMastologiaV2/
├── ControlMastologiaV2/          # Aplicación de escritorio WPF (Producción local)
│   ├── App.xaml / App.xaml.cs   # Inicialización de la base de datos SQLite
│   ├── LoginWindow.xaml         # Ventana de acceso a la consulta
│   ├── PatientRegistrationWindow.xaml # Ficha e historia clínica del paciente
│   └── DiagnosticWindow.xaml    # Evaluación mastológica e informe médico
│
├── ControlMastologia.Web/       # Prototipo inicial de Web API para sincronización
│   ├── Controllers/             # Controladores de prueba (PacientesController)
│   └── Models/                  # Modelos de datos en la nube
│
├── ControlMastologiaV2.slnx     # Solución de Visual Studio
└── README.md                    # Documentación del proyecto
```

---

## ⚙️ Cómo Ejecutar el Proyecto

### Requisitos
* Windows 10 / 11 (64 bits).
* [.NET 10 SDK](https://dotnet.microsoft.com/download) o Visual Studio 2022+ con carga de trabajo para desarrollo de escritorio .NET.

### Pasos
```bash
# 1. Clonar el repositorio
git clone https://github.com/viking21121/control-mastologia-.git
cd control-mastologia-

# 2. Restaurar dependencias y compilar
dotnet restore ControlMastologiaV2.slnx
dotnet build ControlMastologiaV2.slnx -c Release

# 3. Iniciar la aplicación
dotnet run --project ControlMastologiaV2/ControlMastologiaV2.csproj
```

---

## 👤 Autor
* **Josmar Alejandro** — [GitHub: @viking21121](https://github.com/viking21121) — [josmaralejandro21@gmail.com](mailto:josmaralejandro21@gmail.com)
