import React, { useEffect, useState } from 'react';
import { api } from '../../api';
import { toast } from 'react-toastify';
import '../../styles/Common.css';
import './Styles.css';
import LinkModal from './LinkModal';
import Grid from '../../components/Grid';
import DeleteButton from '../../components/DeleteButton';
import type { GridColumn } from '../../components/Grid';
import type { Link } from './types';

const emptyLink: Link = { id: 0, description: '', url: '', comments: '', read: false };

const columns: GridColumn<Link>[] = [
  { header: 'Id', accessor: 'id' },
  { header: 'Description', accessor: 'description' },
  { header: 'Link', accessor: 'url' },
  { header: 'Comments', accessor: 'comments' },
  { header: 'Read', accessor: 'read' },
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

const Links: React.FC = () => {
  const [links, setLinks] = useState<Link[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalLink, setModalLink] = useState<Link>(emptyLink);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [saving, setSaving] = useState(false);
  const [deleteLoading, setDeleteLoading] = useState<number | null>(null);

  useEffect(() => {
    const fetchLinks = async () => {
      try {
        const data = await api.links.list();
        setLinks(data);
      } catch {
        toast.error('Failed to fetch links');
      } finally {
        setLoading(false);
      }
    };
    fetchLinks();
  }, []);

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

  const handleSave = async () => {
    setSaving(true);
    try {
      if (editingId) {
        const updated = await api.links.update(editingId, { 
          description: modalLink.description, 
          url: modalLink.url, 
          comments: modalLink.comments,
          read: modalLink.read
        });
        setLinks(links.map(l => (l.id === editingId ? updated : l)));
        toast.success('Link updated!');
      } else {
        const created = await api.links.create({ 
          description: modalLink.description, 
          url: modalLink.url, 
          comments: modalLink.comments,
          read: modalLink.read
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
        <button className="page-new-btn" onClick={openNewModal}>New</button>
        <h2 className="page-title">Links</h2>
        <div className="page-header-actions"></div>
      </div>
      <Grid
        columns={columns}
        data={links}
        renderActions={link => (
          <>
            <button className="action-btn edit" onClick={() => openEditModal(link)}>Edit</button>
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