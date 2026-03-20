## ADDED Requirements

### Requirement: Studio CRUD operations
The system SHALL provide full CRUD operations for studio locations scoped to the current tenant.

#### Scenario: List studios
- **WHEN** a CompanyUser navigates to the studios index page
- **THEN** the system displays all active studios with their room counts for that tenant

#### Scenario: Create studio
- **WHEN** a CompanyAdmin or CompanyManager submits a valid studio creation form
- **THEN** the system creates the studio with the current tenant's CompanyId

#### Scenario: View studio detail
- **WHEN** a CompanyUser views a studio detail page
- **THEN** the system displays studio information and a list of its rooms

#### Scenario: Update studio
- **WHEN** a CompanyAdmin or CompanyManager submits a valid studio edit form
- **THEN** the system updates the studio if it belongs to the current tenant

#### Scenario: Delete studio
- **WHEN** a CompanyAdmin or CompanyManager requests deletion of a studio
- **THEN** the system performs a soft delete if the studio belongs to the current tenant and has no rooms

#### Scenario: Delete studio with rooms blocked
- **WHEN** a CompanyAdmin or CompanyManager attempts to delete a studio that has rooms
- **THEN** the system prevents deletion and returns an error indicating rooms must be removed first

#### Scenario: Tenant isolation
- **WHEN** a CompanyUser views studios
- **THEN** the system only returns studios where CompanyId matches the current tenant

### Requirement: Studio room management
The system SHALL provide CRUD operations for rooms within a studio.

#### Scenario: List rooms by studio
- **WHEN** a CompanyUser navigates to a studio's rooms page
- **THEN** the system displays all active rooms for that studio

#### Scenario: Create room
- **WHEN** a CompanyAdmin or CompanyManager submits a valid room creation form for a studio
- **THEN** the system creates the room linked to the specified studio

#### Scenario: Update room
- **WHEN** a CompanyAdmin or CompanyManager submits a valid room edit form
- **THEN** the system updates the room if it belongs to a studio in the current tenant

#### Scenario: Delete room
- **WHEN** a CompanyAdmin or CompanyManager requests deletion of a room
- **THEN** the system performs a soft delete if the room belongs to the current tenant

#### Scenario: Add feature to room
- **WHEN** a CompanyAdmin or CompanyManager adds a feature to a room with optional validity period
- **THEN** the system creates a StudioFeature join record linking the room to the feature

#### Scenario: Remove feature from room
- **WHEN** a CompanyAdmin or CompanyManager removes a feature from a room
- **THEN** the system deletes the StudioFeature join record

#### Scenario: View room features
- **WHEN** a CompanyUser views a room edit page
- **THEN** the system displays the room's current features with their validity periods

### Requirement: Feature reference data
The system SHALL provide a shared reference table for physical room features.

#### Scenario: Features seeded
- **WHEN** the database is initialized
- **THEN** the system seeds the Feature table with common values: Sprung Floor, Mirrors, Barres, Poles, Aerial Rigging, Sound System

#### Scenario: Feature selection
- **WHEN** a CompanyAdmin or CompanyManager adds a feature to a room
- **THEN** the system presents a dropdown of all available features from the shared table
