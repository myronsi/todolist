import React, { useState } from 'react';
import { Plus, Circle, CircleCheck, Trash, Funnel } from 'lucide-react';

interface Task {
  id: number;
  text: string;
  completed: boolean;
}

const TodoList: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [newTask, setNewTask] = useState('');
  const [showFilterDropdown, setShowFilterDropdown] = useState(false);
  const [filters, setFilters] = useState({
    open: true,
    completed: true,
  });

  const addTask = () => {
    if (!newTask.trim()) return;

    const newItem: Task = {
      id: Date.now(),
      text: newTask.trim(),
      completed: false,
    };
    setTasks([newItem, ...tasks]);
    setNewTask('');
  };

  const toggleComplete = (id: number) => {
    setTasks(prev =>
      prev.map(task =>
        task.id === id ? { ...task, completed: !task.completed } : task
      )
    );
  };

  const removeTask = (id: number) => {
    setTasks(prev => prev.filter(task => task.id !== id));
  };

  const openTasks = tasks.filter(task => !task.completed);
  const completedTasks = tasks.filter(task => task.completed);

  const handleFilterChange = (filterType: 'open' | 'completed' | 'all') => {
    if (filterType === 'all') {
      setFilters({ open: true, completed: true });
    } else {
      if (
        (filterType === 'open' && !filters.completed && filters.open) ||
        (filterType === 'completed' && !filters.open && filters.completed)
      ) {
        return;
      }
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
                checked={filters.open && filters.completed}
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
                checked={filters.completed}
                onChange={() => handleFilterChange('completed')}
                className="mr-2"
              />
              Completed
            </label>
          </div>
        </div>
      )}
      
      {filters.open && filters.completed && tasks.length === 0 && (
        <p className="text-center text-gray-500">No tasks yet</p>
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
                <span>{task.text}</span>
                <div className="flex items-center space-x-2">
                  <button
                    className="text-red-600 hover:text-red-800"
                    onClick={() => removeTask(task.id)}
                  >
                    <Trash />
                  </button>
                  <button
                    onClick={() => toggleComplete(task.id)}
                    className="text-green-600 hover:text-green-800"
                  >
                    <Circle size={24} />
                  </button>
                </div>
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
                  onClick={() => toggleComplete(task.id)}
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