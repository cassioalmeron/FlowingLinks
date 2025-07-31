import React, { useState, useEffect } from 'react';
import { api } from '../../api';
import './Styles.css';

export interface Tag {
  id: number;
  name: string;
}

interface TagSelectorProps {
  selectedTags: number[];
  onTagsChange: (tagIds: number[]) => void;
  disabled?: boolean;
}

const TagSelector: React.FC<TagSelectorProps> = ({ selectedTags, onTagsChange, disabled = false }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [tags, setTags] = useState<Tag[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchTags = async () => {
      try {
        setLoading(true);
        setError(null);
        const labels = await api.labels.list();
        
        // Convert labels to tags
        const tagsData: Tag[] = labels.map(label => ({
          id: label.id,
          name: label.name
        }));
        
        setTags(tagsData);
      } catch (error: unknown) {
        console.error('Failed to fetch tags:', error);
        setError('Failed to load tags');
      } finally {
        setLoading(false);
      }
    };

    fetchTags();
  }, []);

  const toggleTag = (tagId: number) => {
    if (disabled) return;
    
    const newSelectedTags = selectedTags.includes(tagId)
      ? selectedTags.filter(id => id !== tagId)
      : [...selectedTags, tagId];
    
    onTagsChange(newSelectedTags);
  };

  const getSelectedTagNames = () => {
    return tags
      .filter(tag => selectedTags.includes(tag.id))
      .map(tag => tag.name)
      .join(', ');
  };

  if (loading) {
    return (
      <div className="tag-selector">
        <label>
          Tags:
          <div className="tag-selector-input">
            <span className="tag-selector-display">Loading tags...</span>
            <span className="tag-selector-arrow">▼</span>
          </div>
        </label>
      </div>
    );
  }

  if (error) {
    return (
      <div className="tag-selector">
        <label>
          Tags:
          <div className="tag-selector-input">
            <span className="tag-selector-display">Error loading tags</span>
            <span className="tag-selector-arrow">▼</span>
          </div>
        </label>
      </div>
    );
  }

  return (
    <div className="tag-selector">
      <label>
        Tags:
        <div className="tag-selector-input" onClick={() => !disabled && setIsOpen(!isOpen)}>
          <span className="tag-selector-display">
            {selectedTags.length > 0 ? getSelectedTagNames() : 'Select tags...'}
          </span>
          <span className="tag-selector-arrow">▼</span>
        </div>
      </label>
      
      {isOpen && (
        <div className="tag-selector-dropdown">
          {tags.map(tag => (
            <div
              key={tag.id}
              className={`tag-option ${selectedTags.includes(tag.id) ? 'selected' : ''}`}
              onClick={() => toggleTag(tag.id)}
            >
              <span className="tag-name">{tag.name}</span>
              {selectedTags.includes(tag.id) && (
                <span className="tag-check">✓</span>
              )}
            </div>
          ))}
        </div>
      )}
      
      {selectedTags.length > 0 && (
        <div className="selected-tags">
          {tags
            .filter(tag => selectedTags.includes(tag.id))
            .map(tag => (
              <span
                key={tag.id}
                className="selected-tag"
                onClick={() => !disabled && toggleTag(tag.id)}
              >
                {tag.name} ×
              </span>
            ))}
        </div>
      )}
    </div>
  );
};

export default TagSelector; 