import { Component, OnInit } from '@angular/core';
import { Observer } from 'rxjs';
import { TodoService, TodoItem } from './todo.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  items: TodoItem[] = [];
  newTodoItem: string = '';
  errorMessage: string | null = null;

  public constructor(private todoService: TodoService) {}

  ngOnInit(): void {
    this.getItems();
  }

  getItems() {
    const observer: Observer<TodoItem[]>= {
      next: (data: TodoItem[]) => {
        this.items = data;
        this.errorMessage = null;
      },
      error: (error: string) => {
        this.errorMessage = error;
        this.items = [];
      },
      complete: () => {}
    };

    this.todoService.getTodos().subscribe(observer);
  }

  handleAdd() {
    if (!this.newTodoItem.trim()) {
      return;
    }

    const newTodo: TodoItem = {id: 0, description: this.newTodoItem, isCompleted: false};
    const observer: Observer<TodoItem>= {
      next: (data: TodoItem) => {
        this.items.push(data);
        this.newTodoItem = '';
        this.errorMessage = null;
      },
      error: (error: string) => {
        this.errorMessage = error;
      },
      complete: () => {}
    };

    this.todoService.createTodoItem(newTodo).subscribe(observer);
  }

  handleClear() {
    this.newTodoItem = '';
  }

  handleMarkAsComplete(item: TodoItem) {
    const observer: Observer<void>= {
      next: () => {
        const todo = this.items.find(t => t.id === item.id);
        if (todo) {
          todo.isCompleted = true;
        }
        this.errorMessage = null;
      },
      error: (error: string) => {
        this.errorMessage = error;
      },
      complete: () => {}
    };

    this.todoService.markAsComplete(item.id).subscribe(observer);
  }

  isAddButtonDisabled(): boolean {
    return !this.newTodoItem.trim();
  }
}
