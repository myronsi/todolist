import React, { useState } from 'react';
import { Plus, Circle, CircleCheck, Trash } from 'lucide-react';

interface Task {
  id: number;
  text: string;
  completed: boolean;
}

const TodoList: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [newTask, setNewTask] = useState('');

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
      </div>

      {openTasks.length === 0 ? (
        <p className="text-center text-gray-500">No open tasks yet</p>
      ) : (
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
                    className='text-red-600 hover:text-red-800'
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

      {tasks.some(t => t.completed) && (
        <div className="mt-6">
          <h2 className="text-lg font-semibold mb-2">Completed Tasks</h2>
          <ul className="space-y-2">
            {tasks
              .filter(t => t.completed)
              .map(task => (
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