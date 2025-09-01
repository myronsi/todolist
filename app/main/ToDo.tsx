import React, { useState, useEffect } from 'react';
import { Plus, Circle, CircleCheck, Trash, Funnel, Clock, Edit } from 'lucide-react';

interface Task {
  id: number;
  text: string;
  status: 'open' | 'in-progress' | 'completed';
}

interface Filters {
  open: boolean;
  inProgress: boolean;
  completed: boolean;
}

const TodoList: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>(() => {
    if (typeof window !== 'undefined' && window.localStorage) {
      try {
        const savedTasks = localStorage.getItem('tasks');
        return savedTasks ? JSON.parse(savedTasks) : [];
      } catch (error) {
        return [];
      }
    }
    return [];
  });
  const [newTask, setNewTask] = useState('');
  const [showFilterDropdown, setShowFilterDropdown] = useState(false);
  const [filters, setFilters] = useState<Filters>(() => {
    if (typeof window !== 'undefined' && window.localStorage) {
      try {
        const savedFilters = localStorage.getItem('filters');
        return savedFilters
          ? JSON.parse(savedFilters)
          : { open: true, inProgress: true, completed: true };
      } catch (error) {
        return { open: true, inProgress: true, completed: true };
      }
    }
    return { open: true, inProgress: true, completed: true };
  });
  const [editingTaskId, setEditingTaskId] = useState<number | null>(null);
  const [editText, setEditText] = useState('');

  useEffect(() => {
    if (typeof window !== 'undefined' && window.localStorage) {
      try {
        localStorage.setItem('tasks', JSON.stringify(tasks));
      } catch (error) {
        console.error('Error writing to localStorage:', error);
      }
    }
  }, [tasks]);

  useEffect(() => {
    if (typeof window !== 'undefined' && window.localStorage) {
      try {
        localStorage.setItem('filters', JSON.stringify(filters));
      } catch (error) {
        console.error('Error writing to localStorage:', error);
      }
    }
  }, [filters]);

  const addTask = () => {
    if (!newTask.trim()) return;

    const newItem: Task = {
      id: Date.now(),
      text: newTask.trim(),
      status: 'open',
    };
    setTasks([newItem, ...tasks]);
    setNewTask('');
  };

  const toggleStatus = (id: number) => {
    setTasks(prev =>
      prev.map(task =>
        task.id === id
          ? {
              ...task,
              status:
                task.status === 'open'
                  ? 'in-progress'
                  : task.status === 'in-progress'
                  ? 'completed'
                  : 'open',
            }
          : task
      )
    );
  };

  const removeTask = (id: number) => {
    setTasks(prev => prev.filter(task => task.id !== id));
  };

  const startEditing = (task: Task) => {
    setEditingTaskId(task.id);
    setEditText(task.text);
  };

  const saveEdit = (id: number) => {
    if (!editText.trim()) return;
    setTasks(prev =>
      prev.map(task =>
        task.id === id ? { ...task, text: editText.trim() } : task
      )
    );
    setEditingTaskId(null);
    setEditText('');
  };

  const cancelEdit = () => {
    setEditingTaskId(null);
    setEditText('');
  };

  const openTasks = tasks.filter(task => task.status === 'open');
  const inProgressTasks = tasks.filter(task => task.status === 'in-progress');
  const completedTasks = tasks.filter(task => task.status === 'completed');

  const handleFilterChange = (filterType: 'open' | 'inProgress' | 'completed' | 'all') => {
    if (filterType === 'all') {
      setFilters({ open: true, inProgress: true, completed: true });
    } else {
      const otherFilters = Object.keys(filters).filter(
        key => key !== filterType && key !== 'all'
      );
      const isLastFilter =
        !filters[filterType] &&
        otherFilters.every(key => !filters[key as keyof Filters]);
      if (isLastFilter) return;
      setFilters(prev => ({
        ...prev,
        [filterType]: !prev[filterType],
      }));
    }
  };

  return (
    <div className="max-w-md mx-auto mt-10 p-6 bg-white shadow-md rounded-md">
      <h1 className="text-xs text-center mb-2">Schiftconnector®</h1>
      <h1 className="text-2xl font-bold text-center mb-4">My Todos</h1>

      <div className="flex mb-4">
        <input
          type="text"
          className="flex-1 border border-gray-300 rounded-l px-3 py-2 focus:outline-none focus:ring focus:border-blue-300"
          placeholder="Create a new task..."
          value={newTask}
          onChange={(e) => setNewTask(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && addTask()}
        />
        <button
          className="bg-blue-500 text-white px-4 py-2 rounded-r hover:bg-blue-600"
          onClick={addTask}
        >
          <Plus />
        </button>
        <button
          className="ml-2 bg-gray-200 text-gray-700 px-4 py-2 rounded hover:bg-gray-300"
          onClick={() => setShowFilterDropdown(!showFilterDropdown)}
        >
          <Funnel />
        </button>
      </div>

      {showFilterDropdown && (
        <div className="mb-4 bg-gray-100 p-4 rounded shadow-md">
          <h3 className="text-sm font-semibold mb-2">Filter Tasks</h3>
          <div className="flex flex-col space-y-2">
            <label className="flex items-center">
              <input
                type="checkbox"
                checked={filters.open && filters.inProgress && filters.completed}
                onChange={() => handleFilterChange('all')}
                className="mr-2"
              />
              All
            </label>
            <label className="flex items-center">
              <input
                type="checkbox"
                checked={filters.open}
                onChange={() => handleFilterChange('open')}
                className="mr-2"
              />
              Open
            </label>
            <label className="flex items-center">
              <input
                type="checkbox"
                checked={filters.inProgress}
                onChange={() => handleFilterChange('inProgress')}
                className="mr-2"
              />
              In Progress
            </label>
            <label className="flex items-center">
              <input
                type="checkbox"
                checked={filters.completed}
                onChange={() => handleFilterChange('completed')}
                className="mr-2"
              />
              Completed
            </label>
          </div>
        </div>
      )}

      {filters.open && filters.inProgress && filters.completed && tasks.length === 0 && (
        <p className="text-center text-gray-500">No tasks yet</p>
      )}
      {filters.open && openTasks.length === 0 && !filters.inProgress && !filters.completed && (
        <p className="text-center text-gray-500">No open tasks yet</p>
      )}
      {filters.inProgress && inProgressTasks.length === 0 && !filters.open && !filters.completed && (
        <p className="text-center text-gray-500">No in-progress tasks yet</p>
      )}
      {filters.completed && completedTasks.length === 0 && !filters.open && !filters.inProgress && (
        <p className="text-center text-gray-500">No completed tasks yet</p>
      )}

      {filters.open && openTasks.length > 0 && (
        <div className="mt-3">
          <h2 className="text-lg font-semibold mb-2">Open Tasks</h2>
          <ul className="space-y-2">
            {openTasks.map(task => (
              <li
                key={task.id}
                className="flex items-center justify-between bg-gray-100 p-2 rounded"
              >
                {editingTaskId === task.id ? (
                  <div className="flex flex-1 items-center space-x-2">
                    <input
                      type="text"
                      className="flex-1 border border-gray-300 px-2 py-1 rounded focus:outline-none focus:ring focus:border-blue-300"
                      value={editText}
                      onChange={(e) => setEditText(e.target.value)}
                      onKeyDown={(e) => e.key === 'Enter' && saveEdit(task.id)}
                    />
                    <button
                      className="text-blue-600 hover:text-blue-800"
                      onClick={() => saveEdit(task.id)}
                    >
                      Save
                    </button>
                    <button
                      className="text-gray-600 hover:text-gray-800"
                      onClick={cancelEdit}
                    >
                      Cancel
                    </button>
                  </div>
                ) : (
                  <>
                    <span>{task.text}</span>
                    <div className="flex items-center space-x-2">
                      <button
                        className="text-blue-600 hover:text-blue-800"
                        onClick={() => startEditing(task)}
                      >
                        <Edit size={24} />
                      </button>
                      <button
                        className="text-red-600 hover:text-red-800"
                        onClick={() => removeTask(task.id)}
                      >
                        <Trash size={24} />
                      </button>
                      <button
                        onClick={() => toggleStatus(task.id)}
                        className="text-green-600 hover:text-green-800"
                      >
                        <Circle size={24} />
                      </button>
                    </div>
                  </>
                )}
              </li>
            ))}
          </ul>
        </div>
      )}

      {filters.inProgress && inProgressTasks.length > 0 && (
        <div className="mt-6">
          <h2 className="text-lg font-semibold mb-2">In Progress Tasks</h2>
          <ul className="space-y-2">
            {inProgressTasks.map(task => (
              <li
                key={task.id}
                className="flex items-center justify-between bg-gray-100 p-2 rounded"
              >
                {editingTaskId === task.id ? (
                  <div className="flex flex-1 items-center space-x-2">
                    <input
                      type="text"
                      className="flex-1 border border-gray-300 px-2 py-1 rounded focus:outline-none focus:ring focus:border-blue-300"
                      value={editText}
                      onChange={(e) => setEditText(e.target.value)}
                      onKeyDown={(e) => e.key === 'Enter' && saveEdit(task.id)}
                    />
                    <button
                      className="text-blue-600 hover:text-blue-800"
                      onClick={() => saveEdit(task.id)}
                    >
                      Save
                    </button>
                    <button
                      className="text-gray-600 hover:text-gray-800"
                      onClick={cancelEdit}
                    >
                      Cancel
                    </button>
                  </div>
                ) : (
                  <>
                    <span>{task.text}</span>
                    <div className="flex items-center space-x-2">
                      <button
                        className="text-blue-600 hover:text-blue-800"
                        onClick={() => startEditing(task)}
                      >
                        <Edit size={24} />
                      </button>
                      <button
                        className="text-red-600 hover:text-red-800"
                        onClick={() => removeTask(task.id)}
                      >
                        <Trash size={24} />
                      </button>
                      <button
                        onClick={() => toggleStatus(task.id)}
                        className="text-yellow-600 hover:text-yellow-800"
                      >
                        <Clock size={24} />
                      </button>
                    </div>
                  </>
                )}
              </li>
            ))}
          </ul>
        </div>
      )}

      {filters.completed && completedTasks.length > 0 && (
        <div className="mt-6">
          <h2 className="text-lg font-semibold mb-2">Completed Tasks</h2>
          <ul className="space-y-2">
            {completedTasks.map(task => (
              <li
                key={task.id}
                className="flex items-center justify-between bg-gray-100 p-2 rounded"
              >
                <span>{task.text}</span>
                <button
                  onClick={() => toggleStatus(task.id)}
                  className="text-green-600 hover:text-green-800"
                >
                  <CircleCheck size={24} />
                </button>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
};

export default TodoList;
