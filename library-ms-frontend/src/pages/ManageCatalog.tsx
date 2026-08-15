import { useState, useEffect } from 'react';
import api from '../api';
import AddAuthorForm from '../components/AddAuthorForm';
import AddCategoryForm from '../components/AddCategoryForm';
import AddLocationForm from '../components/AddLocationForm';
import AddBookForm from '../components/AddBookForm';
import ManageList from '../components/ManageList';
import { Author, Category, Location, Book } from '../types';

export default function ManageCatalog() {
  const [authors, setAuthors] = useState<Author[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [locations, setLocations] = useState<Location[]>([]);
  const [books, setBooks] = useState<Book[]>([]);

  const refresh = () => {
    api.get('/Authors').then((r) => setAuthors(r.data));
    api.get('/Categories').then((r) => setCategories(r.data));
    api.get('/Locations').then((r) => setLocations(r.data));
    api.get('/Books').then((r) => setBooks(r.data));
  };

  useEffect(refresh, []);

  return (
    <div className="page-content">
      <h2>Manage Catalog</h2>

      <AddAuthorForm onAdded={refresh} />
      <ManageList
        title="Existing Authors"
        endpoint="/Authors"
        items={authors}
        getId={(a) => a.id}
        renderLabel={(a) => a.fullName}
        onDeleted={(id) => setAuthors((prev) => prev.filter((a) => a.id !== id))}
      />
      <hr style={{ margin: '2rem 0', border: 'none', borderTop: '1px solid var(--border)' }} />

      <AddCategoryForm onAdded={refresh} />
      <ManageList
        title="Existing Categories"
        endpoint="/Categories"
        items={categories}
        getId={(c) => c.id}
        renderLabel={(c) => c.name}
        onDeleted={(id) => setCategories((prev) => prev.filter((c) => c.id !== id))}
      />
      <hr style={{ margin: '2rem 0', border: 'none', borderTop: '1px solid var(--border)' }} />

      <AddLocationForm onAdded={refresh} />
      <ManageList
        title="Existing Locations"
        endpoint="/Locations"
        items={locations}
        getId={(l) => l.id}
        renderLabel={(l) => l.name}
        onDeleted={(id) => setLocations((prev) => prev.filter((l) => l.id !== id))}
      />
      <hr style={{ margin: '2rem 0', border: 'none', borderTop: '1px solid var(--border)' }} />

      <AddBookForm onAdded={refresh} />
      <ManageList
        title="Existing Books"
        endpoint="/Books"
        items={books}
        getId={(b) => b.id}
        renderLabel={(b) => `${b.title} (${b.authorName})`}
        onDeleted={(id) => setBooks((prev) => prev.filter((b) => b.id !== id))}
      />
    </div>
  );
}