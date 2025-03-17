export interface AircraftModel {
    id: number;
    name: string;
    manufacturer: string;
    model: string;
    cruiseSpeed: number;
    range: number;
    maxCrosswind: number;
}

export interface AircraftListModel {
    data: AircraftModel[];
    totalRecords: number;
}