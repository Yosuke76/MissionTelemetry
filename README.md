# MissionTelemetry

Kleine Demo-Anwendung zur Weltraum-Telemetrie mit **WPF**:
- Live-Simulation von Housekeeping-Daten (Spannung, Temperatur, SNR, Attitude).
- Proximity/Radar-Ansicht mit **Velocity-Pfeilen**, CPA/TCPA-Schätzung.
- Alarmsystem mit **Ampel**, Warning/Alarm, **Latching** und **Ack**.

## Projekte
- `MissionTelemetry.Core` – Domänenmodell, Simulation, Evaluator, AlarmManager, Proximity.
- `MissionTelemetry.Wpf`  – WPF-UI (Radar, Telemetrie-Grid, Active Alarms, Star-Wars-Style).
- (geplant) `MissionTelemetry.Api` – ASP.NET Core Web API.

## Geplante Erweiterungen (Projektwoche)
1. **ASP.NET Core Web API**
   - Endpoints:  
     - `GET /api/telemetry/latest?take=`  
     - `GET /api/proximity`  
     - `GET /api/alarms`  
     - `POST /api/alarms/ack/{id}`, `POST /api/alarms/ack-all`  
     - `POST /api/sim/start|stop|clear`
   - CORS + Swagger aktiv.

2. **Data-Driven AlarmEvaluator**
   - Limits/Hysterese/Persistenz aus `mission_dict.json` (JSON).
   - Austauschbar über `IAlarmEvaluator`, UI unverändert.

3. **Persistenz via EF Core (SQLite)**
   - Tabellen: `TelemetryFrames`, `ProximitySnapshots`, `AlarmAudit`.
   - Migrations: `dotnet ef migrations add Init` → `dotnet ef database update`.

