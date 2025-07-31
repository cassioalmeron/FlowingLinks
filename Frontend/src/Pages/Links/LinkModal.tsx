import React, { useState } from 'react';
import * as yup from 'yup';
import '../../styles/Common.css';
import './LinkModalStyles.css';
import TagSelector from '../../components/TagSelector';
import type { Link } from './types';

interface LinkModalProps {
  open: boolean;
  link: Link;
  loading: boolean;
  editingId: number | null;
  onChange: (link: Link) => void;
  onSave: () => void;
  onClose: () => void;
}

const linkSchema = yup.object({
  description: yup.string().required('Description is required').min(2, 'Description must be at least 2 characters'),
  url: yup.string().required('Link is required').url('Link must be a valid URL'),
  comments: yup.string().optional(),
  read: yup.boolean().required('Read status is required'),
});

const LinkModal: React.FC<LinkModalProps> = ({ open, link, loading, editingId, onChange, onSave, onClose }) => {
  const [errors, setErrors] = useState<{ description?: string; url?: string; comments?: string; read?: string }>({});

  if (!open) return null;

  const validate = async () => {
    try {
      await linkSchema.validate(link, { abortEarly: false });
      setErrors({});
      return true;
    } catch (err) {
      if (err instanceof yup.ValidationError) {
        const newErrors: { description?: string; link?: string; comments?: string; read?: string } = {};
        err.inner.forEach((e) => {
          if (e.path) newErrors[e.path as keyof typeof newErrors] = e.message;
        });
        setErrors(newErrors);
      }
      return false;
    }
  };

  const handleKeyDown = async (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !loading) {
      e.preventDefault();
      if (await validate()) onSave();
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!loading && (await validate())) {
      onSave();
    }
  };

  const handleTagsChange = (tagIds: number[]) => {
    onChange({ ...link, tags: tagIds });
  };

  return (
    <div className="common-modal-backdrop">
      <div className="common-modal">
        <h3>{editingId ? 'Edit Link' : 'New Link'}</h3>
        <form className="common-modal-form" onSubmit={handleSubmit} onKeyDown={handleKeyDown}>
          <label>
            Description:
            <input
              type="text"
              value={link.description}
              onChange={e => onChange({ ...link, description: e.target.value })}
              disabled={loading}
            />
            {errors.description && <div className="login-error">{errors.description}</div>}
          </label>
          <label>
            Link:
            <input
              type="url"
              value={link.url}
              onChange={e => onChange({ ...link, url: e.target.value })}
              disabled={loading}
              placeholder="https://example.com"
            />
            {errors.url && <div className="login-error">{errors.url}</div>}
          </label>
          <TagSelector
            selectedTags={link.tags || []}
            onTagsChange={handleTagsChange}
            disabled={loading}
          />
          <label>
            Comments:
            <textarea
              value={link.comments || ''}
              onChange={e => onChange({ ...link, comments: e.target.value })}
              disabled={loading}
              placeholder="Optional comments about this link"
              rows={3}
            />
            {errors.comments && <div className="login-error">{errors.comments}</div>}
          </label>
          <label className="checkbox-label">
            <input
              type="checkbox"
              checked={link.read}
              onChange={e => onChange({ ...link, read: e.target.checked })}
              disabled={loading}
            />
            Read
            {errors.read && <div className="login-error">{errors.read}</div>}
          </label>
        </form>
        <div className="common-modal-actions">
          <button onClick={onClose} className="common-modal-cancel" disabled={loading}>Cancel</button>
          <button onClick={async (e) => { e.preventDefault(); if (await validate()) onSave(); }} className="common-modal-save" disabled={loading}>
            {loading ? 'Saving...' : 'Save'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default LinkModal; 