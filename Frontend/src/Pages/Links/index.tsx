import React, { useEffect, useState } from 'react';
import { api } from '../../api';
import { toast } from 'react-toastify';
import '../../styles/Common.css';
import './Styles.css';
import LinkModal from './LinkModal';
import Grid from '../../components/Grid';
import DeleteButton from '../../components/DeleteButton';
import EditButton from '../../components/EditButton';
import NewButton from '../../components/NewButton';
import TagSelector from '../../components/TagSelector';
import type { GridColumn } from '../../components/Grid';
import type { Link } from './types';

const emptyLink: Link = { id: 0, description: '', url: '', comments: '', read: false, tags: [], favorite: false };

interface Tag {
  id: number;
  name: string;
}

const Links: React.FC = () => {
  const [links, setLinks] = useState<Link[]>([]);
  const [tags, setTags] = useState<Tag[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalLink, setModalLink] = useState<Link>(emptyLink);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [saving, setSaving] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState<number | null>(null);
  
  // Filter states
  const [descriptionFilter, setDescriptionFilter] = useState('');
  const [selectedTagsFilter, setSelectedTagsFilter] = useState<number[]>([]);
  const [favoritesFilter, setFavoritesFilter] = useState<boolean | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [linksData, tagsData] = await Promise.all([
          api.links.list(),
          api.labels.list()
        ]);
        setLinks(linksData);
        setTags(tagsData);
      } catch {
        toast.error('Failed to fetch data');
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  const columns: GridColumn<Link>[] = [
    { header: 'Id', accessor: 'id' },
    { header: 'Description', accessor: 'description' },
    //{ header: 'Link', accessor: 'url' },
    
    { header: 'Comments', accessor: 'comments' },
    { 
      header: 'Favorite', 
      accessor: 'favorite',
      render: (favorite, link) => (
        <button 
          className={`favorite-btn ${favorite ? 'favorited' : ''}`}
          onClick={() => handleToggleFavorite(link)}
          title={favorite ? 'Remove from favorites' : 'Add to favorites'}
        >
          ⭐
        </button>
      )
    },
    { header: 'Read', accessor: 'read' },
    { 
      header: 'Tags', 
      accessor: 'tags',
      render: (linkTags) => {
        if (!linkTags || (linkTags as number[]).length === 0) return '-';
        return (
          <div className="link-tags">
            {(linkTags as number[]).map(tagId => {
              const tag = tags.find((t: Tag) => t.id === tagId);
              return tag ? (
                <span
                  key={tagId}
                  className="link-tag"
                >
                  {tag.name}
                </span>
              ) : null;
            })}
          </div>
        );
      }
    },
    { 
      header: 'Open', 
      accessor: 'url',
      render: (url) => (
        <button 
          className="link-open-btn"
          onClick={() => window.open(url as string, '_blank')}
          title={url as string}
        >
          🔗
        </button>
      )
    },
  ];

  const openNewModal = () => {
    setModalLink({ ...emptyLink });
    setEditingId(null);
    setModalOpen(true);
  };

  const openEditModal = (link: Link) => {
    setModalLink({ ...link });
    setEditingId(link.id);
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setModalLink(emptyLink);
    setEditingId(null);
  };

  const handleToggleFavorite = async (link: Link) => {
    try {
      const newFavoriteStatus = !link.favorite;
      await api.links.toggleFavorite(link.id, newFavoriteStatus);
      setLinks(links.map(l => (l.id === link.id ? { ...l, favorite: newFavoriteStatus } : l)));
    } catch {
      toast.error('Failed to update favorite status');
    }
  };

  const handleSave = async () => {
    setSaving(true);
    try {
      if (editingId) {
        const updated = await api.links.update(editingId, { 
          description: modalLink.description, 
          url: modalLink.url, 
          comments: modalLink.comments,
          read: modalLink.read,
          tags: modalLink.tags,
          favorite: modalLink.favorite
        });
        setLinks(links.map(l => (l.id === editingId ? updated : l)));
        toast.success('Link updated!');
      } else {
        const created = await api.links.create({ 
          description: modalLink.description, 
          url: modalLink.url, 
          comments: modalLink.comments,
          read: modalLink.read,
          tags: modalLink.tags,
          favorite: modalLink.favorite
        });
        setLinks([...links, created]);
        toast.success('Link created!');
      }
      closeModal();
    } catch {
      toast.error('Failed to save link');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id: number) => {
    setDeleteLoading(id);
    try {
      await api.links.delete(id);
      setLinks(links.filter(link => link.id !== id));
      toast.success('Link deleted successfully!');
    } catch {
      toast.error('Failed to delete link');
    } finally {
      setDeleteLoading(null);
    }
  };

  const handleSearch = async () => {
    try {
      setLoading(true);
      
      // Convert favorites filter to API format
      let favoriteValue: number | undefined;
      if (favoritesFilter === true) {
        favoriteValue = 1; // Favorites only
      } else if (favoritesFilter === false) {
        favoriteValue = 2; // Non-favorites only
      } else {
        favoriteValue = 0; // All
      }
      
      const filters = {
        description: descriptionFilter || undefined,
        labelIds: selectedTagsFilter.length > 0 ? selectedTagsFilter : undefined,
        favorite: favoriteValue
      };
      
      const searchResults = await api.links.search(filters);
      setLinks(searchResults);
      
    } catch (error) {
      console.error('Search error:', error);
      toast.error('Failed to search links');
    } finally {
      setLoading(false);
    }
  };

  const clearFilters = async () => {
    setDescriptionFilter('');
    setSelectedTagsFilter([]);
    setFavoritesFilter(null);
    
    // Reload all links when filters are cleared
    try {
      setLoading(true);
      const linksData = await api.links.list();
      setLinks(linksData);
    } catch (error) {
      console.error('Error loading links:', error);
      toast.error('Failed to load links');
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>Loading links...</div>;

  return (
    <div>
      <div className="page-header">
        <NewButton onClick={openNewModal} />
        <h2 className="page-title">Links</h2>
        <div className="page-header-actions"></div>
              </div>
        
        {/* Filter Section */}
        <div className="filters-section">
         <div className="filters-row">
           <div className="filter-group">
             <label htmlFor="description-filter">Description:</label>
             <input
               id="description-filter"
               type="text"
               value={descriptionFilter}
               onChange={(e) => setDescriptionFilter(e.target.value)}
               placeholder="Search in description..."
               className="filter-input"
             />
           </div>
           
           <div className="filter-group">
             <label>Labels:</label>
             <TagSelector
               selectedTags={selectedTagsFilter}
               onTagsChange={setSelectedTagsFilter}
               hideLabel={true}
             />
           </div>
           
           <div className="filter-group">
             <label>Favorites:</label>
             <select
               value={favoritesFilter === null ? '' : favoritesFilter.toString()}
               onChange={(e) => setFavoritesFilter(e.target.value === '' ? null : e.target.value === 'true')}
               className="filter-select"
             >
               <option value="">All</option>
               <option value="true">Favorites only</option>
               <option value="false">Non-favorites only</option>
             </select>
           </div>
           
           <div className="filters-actions">
             <button onClick={handleSearch} className="search-btn">
               🔍 Search
             </button>
             <button onClick={clearFilters} className="clear-btn">
               Clear Filters
             </button>
           </div>
         </div>
       </div>
      
      <Grid
        columns={columns}
        data={links}
        renderActions={link => (
          <>
            <EditButton onClick={() => openEditModal(link)} />
            <DeleteButton
              onConfirm={() => handleDelete(link.id)}
              loading={deleteLoading === link.id}
              confirmMessage={<p>Are you sure you want to delete this link?</p>}
            />
          </>
        )}
      />
      <LinkModal
        open={modalOpen}
        link={modalLink}
        loading={saving}
        editingId={editingId}
        onChange={setModalLink}
        onSave={handleSave}
        onClose={closeModal}
      />
    </div>
  );
};

export default Links;