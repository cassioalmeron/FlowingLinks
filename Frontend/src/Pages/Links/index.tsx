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
    { header: 'Link', accessor: 'url' },
    
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
          title="Open link in new tab"
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

  if (loading) return <div>Loading links...</div>;

  return (
    <div>
      <div className="page-header">
        <NewButton onClick={openNewModal} />
        <h2 className="page-title">Links</h2>
        <div className="page-header-actions"></div>
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