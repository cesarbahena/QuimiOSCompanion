# QuimiOSCompanion

WPF desktop application for laboratory technicians working with legacy LIMS system.

## Features

### Lista de Trabajo (Work List)
View and search sample status in real-time from QuimiOSHub API.

### Entrega de Turno (Shift Handover)
Create shift handover records with pending sample accountability.

### Inventario (Inventory Management)
Automated reagent consumption tracking with PDF parsing capabilities.

- Parse daily consumption PDFs from laboratory equipment
- Select performed calibrations to calculate total consumption
- Validate inventory stock before submission
- Submit consumption records to QuimiOSHub API
- Automatic inventory updates via API

## Technical Stack

- .NET 6 Windows Desktop
- WPF with MVVM pattern
- iText7 for PDF parsing
- REST API integration with QuimiOSHub

## Requirements

- Windows 10 or later
- .NET 6 Runtime
- QuimiOSHub API access