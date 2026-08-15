import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import L from 'leaflet';
import { LocationStats } from '../types';

// Fix Leaflet's default marker icon paths (a known quirk with bundlers like Vite)
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

const defaultIcon = L.icon({
  iconUrl: icon,
  shadowUrl: iconShadow,
  iconSize: [25, 41],
  iconAnchor: [12, 41],
});

interface LocationMapProps {
  locations: LocationStats[];
}

export default function LocationMap({ locations }: LocationMapProps) {
  if (locations.length === 0) {
    return <p style={{ color: 'var(--text2)' }}>No locations added yet.</p>;
  }

  const center: [number, number] = [locations[0].latitude, locations[0].longitude];

  return (
    <div style={{ height: '400px', borderRadius: '10px', overflow: 'hidden', border: '1px solid var(--border)' }}>
      <MapContainer center={center} zoom={4} style={{ height: '100%', width: '100%' }}>
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        {locations.map((loc) => (
          <Marker key={loc.locationId} position={[loc.latitude, loc.longitude]} icon={defaultIcon}>
            <Popup>
              <strong>{loc.name}</strong>
              <br />
              {loc.bookCount} book(s)
            </Popup>
          </Marker>
        ))}
      </MapContainer>
    </div>
  );
}