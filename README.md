# dotNet5785 — Volunteer Call Management System
**Project IDs:** 0199 | 9412  
**Authors:** Sara, Hila

---

## Table of Contents

- [What This Project Is](#what-this-project-is)
- [Architecture Overview](#architecture-overview)
- [Project Structure](#project-structure)
- [Data Entities](#data-entities)
- [Data Layer (DAL)](#data-layer-dal)
- [Business Logic Layer (BL)](#business-logic-layer-bl)
- [Presentation Layer (PL)](#presentation-layer-pl)
- [Test Projects](#test-projects)
- [XML Files](#xml-files)
- [Enums](#enums)
- [How to Run](#how-to-run)

---

## What This Project Is

A C# / WPF desktop application for managing volunteer emergency calls — something like a dispatch system for a medical rescue organization.

The admin opens calls, volunteers take them, and the system tracks who handled what and how it ended. There's also a simulated system clock for testing time-dependent behavior without waiting in real time.

The whole thing is split into three layers (DAL, BL, PL) and has two interchangeable storage backends — in-memory lists and XML files.

---

## Architecture Overview

```
PL  (WPF — windows, XAML, converters)
 |
 |  uses BlApi
 |
BL  (business logic — validation, status computation, assignments)
 |
 |  uses DalApi
 |
DalFacade  (interfaces + data objects — shared by both implementations)
 |              |
DalList      DalXml
(in-memory)  (XML files)
```

Each layer only talks to the one directly below it. The active DAL implementation is picked at startup from `dal-config.xml` via `DalApi.Factory`.

---

## Project Structure

```
dotNet5785_0199_9412/
│
├── dotNet5785_ 0199_9412.sln
├── README.md
├── .gitignore
│
├── Stage0/                          # Stage 0 — initial hello-world console app
│   ├── Program0199.cs
│   ├── Program9412.cs
│   └── Stage0.csproj
│
├── DalFacade/                       # Shared DAL interfaces and data objects
│   ├── DalFacade.csproj
│   ├── DO/                          # Data objects (pure immutable records)
│   │   ├── Volunteer.cs
│   │   ├── Call.cs
│   │   ├── Assignment.cs
│   │   ├── Enums.cs
│   │   └── Exceptions.cs
│   └── DalApi/                      # DAL interfaces
│       ├── IDal.cs
│       ├── IVolunteer.cs
│       ├── ICall.cs
│       ├── IAssignment.cs
│       ├── IConfig .cs
│       ├── ICrud.cs
│       ├── DalConfig.cs
│       └── Factory.cs
│
├── DalList/                         # DAL — in-memory List implementation
│   ├── DalList.csproj
│   ├── DalList.cs
│   ├── VolunteerImplementation.cs
│   ├── CallImplementation.cs
│   ├── AssignmentImplementation.cs
│   ├── ConfigImplementation.cs
│   ├── Config.cs
│   └── DataSource .cs
│
├── DalXml/                          # DAL — XML file implementation
│   ├── DalXml.csproj
│   ├── DalXml.cs
│   ├── VolunteerImplementation.cs
│   ├── CallImplementation.cs
│   ├── AssignmentImplementation.cs
│   ├── ConfigImplementation.cs
│   ├── Config.cs
│   └── XmlTools.cs
│
├── BL/                              # Business logic layer
│   ├── BL.csproj
│   ├── BlApi/                       # BL interfaces (what PL sees)
│   │   ├── IBl .cs
│   │   ├── IVolunteer.cs
│   │   ├── ICall.cs
│   │   ├── IAdmin.cs
│   │   ├── IObservable.cs
│   │   └── Factory.cs
│   ├── BlImplementation/            # Actual BL logic
│   │   ├── Bl.cs
│   │   ├── VolunteerImplementation.cs
│   │   ├── CallImplementation.cs
│   │   └── AdminImplementation.cs
│   ├── BO/                          # Business objects (richer than DO)
│   │   ├── Volunteer.cs
│   │   ├── Call.cs
│   │   ├── CallInList.cs
│   │   ├── CallInProgress.cs
│   │   ├── OpenCallInList.cs
│   │   ├── ClosedCallInList.cs
│   │   ├── CallAssignInList.cs
│   │   ├── VolunteerInList.cs
│   │   ├── Enums.cs
│   │   └── Exceptions.cs
│   └── Helpers/                     # Internal helper managers
│       ├── AdminManager.cs
│       ├── CallManager.cs
│       ├── VolunteerManager.cs
│       ├── AssignmentManager.cs
│       ├── ObserverManager.cs
│       └── Tools.cs
│
├── PL/                              # WPF presentation layer
│   ├── PL.csproj
│   ├── App.xaml / App.xaml.cs
│   ├── MainWindow.xaml / .cs
│   ├── Login.xaml / .cs
│   ├── Converters.cs
│   ├── Enums.cs
│   ├── AssemblyInfo.cs
│   ├── Call/
│   │   ├── CallListWindow.xaml / .cs
│   │   ├── CallWindow.xaml / .cs
│   │   ├── ClosedCallsWindow.xaml / .cs
│   │   └── OpenCallsWindow.xaml / .cs
│   ├── Volunteer/
│   │   ├── VolunteerListWindow.xaml / .cs
│   │   ├── VolunteerWindow.xaml / .cs
│   │   └── VolunteerProfile.xaml / .cs
│   └── Images/
│
├── DalTest/                         # Console app for testing the DAL
│   ├── DalTest.csproj
│   ├── Program.cs
│   └── Initialization.cs
│
├── BlTest/                          # Console app for testing the BL
│   ├── BlTest.csproj
│   └── Program.cs
│
└── xml/                             # XML data files (used by DalXml)
    ├── volunteers.xml
    ├── calls.xml
    ├── assignments.xml
    ├── dal-config.xml
    └── data-config.xml
```

---

## Data Entities

All three core entities are defined as C# `record` types in the `DO` namespace under `DalFacade`.

### Volunteer

| Field | Type | Notes |
|---|---|---|
| Id | int | National ID number |
| Name | string | Full name |
| Phone | string | Phone number |
| Mail | string | Email |
| Password | string? | Used for login |
| Address | string? | Home address |
| Latitude | double? | Geocoded from address |
| Longitude | double? | Geocoded from address |
| Active | bool | Whether they can receive calls |
| MaximumDistance | double? | Max travel distance in km |
| Role | Roles | Volunteer or Manager |
| Type | DistanceType | Aerial, Car, or Walking |

### Call

| Field | Type | Notes |
|---|---|---|
| Id | int | Auto-incremented |
| Description | string? | What the call is about |
| Address | string | Where to go |
| Latitude | double? | Geocoded |
| Longitude | double? | Geocoded |
| OpenTime | DateTime | When the call was created |
| MaxTime | DateTime? | Deadline for handling |
| CarTypeToSend | CallType | Type of vehicle needed |

### Assignment

| Field | Type | Notes |
|---|---|---|
| Id | int | Auto-incremented |
| CallId | int | Which call |
| VolunteerId | int | Which volunteer |
| EnterTime | DateTime | When they accepted |
| EndTime | DateTime? | When they finished |
| TypeEndOfTreatment | EndType? | How it ended |

---

## Data Layer (DAL)

The DAL is behind the `IDal` interface. Both implementations support the same operations — swapping between them requires only a config change.

### DalList

Keeps everything in memory using `List<T>`. Nothing persists between runs. Good for quick testing.

### DalXml

Reads and writes XML files from the `/xml` folder. Data survives restarts. Uses `XmlTools.cs` for all serialization.

### Switching

Open `/xml/dal-config.xml` and change the value:

```xml
<DalConfig>
  <Active>DalXml</Active>
</DalConfig>
```

Set it to `DalList` or `DalXml`.

### Generic CRUD (`ICrud<T>`)

All entity interfaces inherit from `ICrud<T>`, which defines:

- `Create(T item)`
- `Read(int id)`
- `ReadAll(Func<T, bool>? filter)`
- `Update(T item)`
- `Delete(int id)`
- `DeleteAll()`

---

## Business Logic Layer (BL)

The BL is accessed through `BlApi.IBl`, which groups three sub-interfaces:

| Interface | Handles |
|---|---|
| IVolunteer | Volunteer CRUD, login, filtering, sorting |
| ICall | Call CRUD, status transitions, assignment logic |
| IAdmin | Clock control, configuration, system reset |

### Business Objects (BO)

The BL defines its own object types that the PL actually works with. These are built by aggregating DO data and computing derived fields (like call status):

- `BO.Call` — full call with computed status
- `BO.CallInList` — lightweight summary for list views
- `BO.CallInProgress` — a call currently being handled by a volunteer
- `BO.OpenCallInList` — open calls a volunteer can take
- `BO.ClosedCallInList` — calls a volunteer has already closed
- `BO.Volunteer` — full volunteer details
- `BO.VolunteerInList` — lightweight for list views

### Helpers

The `Helpers` folder contains internal manager classes that keep `BlImplementation` from getting too long:

- `AdminManager.cs` — clock advancement, config read/write
- `CallManager.cs` — status computation, risk detection, filtering
- `VolunteerManager.cs` — validation, distance calculation
- `AssignmentManager.cs` — linking volunteers to calls
- `ObserverManager.cs` — observer pattern for live UI updates
- `Tools.cs` — shared utilities

### Observer Pattern

`IObservable` is implemented in the BL so the WPF windows can subscribe to data changes and refresh automatically without polling.

---

## Presentation Layer (PL)

WPF application. All UI is defined in XAML with C# code-behind.

### Windows

| Window | What it does |
|---|---|
| Login.xaml | Entry point — login for both managers and volunteers |
| MainWindow.xaml | Admin dashboard, clock display, navigation |
| CallListWindow.xaml | Table of all calls, admin can add/edit |
| CallWindow.xaml | Add or edit a single call |
| OpenCallsWindow.xaml | Volunteer view — calls they can take |
| ClosedCallsWindow.xaml | Volunteer view — their history |
| VolunteerListWindow.xaml | Admin view of all volunteers |
| VolunteerWindow.xaml | Add or edit a volunteer |
| VolunteerProfile.xaml | Volunteer's own profile and current assignment |

### Converters.cs

Contains `IValueConverter` implementations used in XAML bindings — things like converting an enum value to a readable string, or a boolean to `Visibility`.

---

## Test Projects

### DalTest

Console app. Run it to test the DAL layer directly.

- `Initialization.cs` seeds the database with sample data
- `Program.cs` gives an interactive menu for all CRUD operations

### BlTest

Console app. Run it to test the BL layer.

- `Program.cs` gives an interactive menu covering volunteer login, call assignment, status changes, clock control, etc.

---

## XML Files

All stored under `/xml`:

| File | What's in it |
|---|---|
| volunteers.xml | Volunteer records |
| calls.xml | Call records |
| assignments.xml | Assignment records |
| dal-config.xml | Which DAL is active |
| data-config.xml | System clock, risk thresholds, other config |

---

## Enums

### Defined in `DO` (used across DAL and BL)

| Enum | Values |
|---|---|
| Roles | Volunteer, Manager |
| DistanceType | Aerial, Car, Walking |
| CallType | RegularVehicle, Ambulance, IntensiveCareAmbulance |
| CallStatus | Open, Treatment, OpenAtRisk, TreatmentOfRisk, Expired, Close |
| EndType | Treated, AdminCancellation, SelfCancellation, ExpiredCancellation |

### Used in test console menus

| Enum | Purpose |
|---|---|
| MainMenuOptions | Top-level menu |
| EntityMenuOptions | CRUD submenu |
| ConfigMenuOptions | Clock / config submenu |

---

## How to Run

**Requirements:** .NET 8 SDK, Visual Studio 2022 with the WPF workload.

### Full Application

1. Open `dotNet5785_ 0199_9412.sln` in Visual Studio
2. Set `PL` as the startup project
3. Build (`Ctrl+Shift+B`) and run (`F5`)

### DAL / BL Tests

Right-click `DalTest` or `BlTest`, set as startup project, and run. You'll get an interactive console menu.

---

## Projects in the Solution

| Project | Type | Role |
|---|---|---|
| Stage0 | Console App | Stage 0 hello-world |
| DalFacade | Class Library | Interfaces and DO models |
| DalList | Class Library | In-memory DAL |
| DalXml | Class Library | XML-based DAL |
| BL | Class Library | Business logic |
| PL | WPF App | Desktop UI |
| DalTest | Console App | DAL tests |
| BlTest | Console App | BL tests |