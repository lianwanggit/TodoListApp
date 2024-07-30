import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { TodoService } from './todo.service';
import { of, throwError } from 'rxjs'

describe('AppComponent', () => {
    let component: AppComponent;
    let fixture: ComponentFixture<AppComponent>;
    let todoService: TodoService;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AppComponent],
            imports: [HttpClientTestingModule, FormsModule],
            providers: [TodoService]
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(AppComponent);
        component = fixture.componentInstance;
        todoService = TestBed.inject(TodoService);
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should fetch todos on init', () => {
        const todos = [{id: 1, description: 'Test Todo', isCompleted: false }];
        spyOn(todoService, 'getTodos').and.returnValue(of(todos));

        component.ngOnInit();

        expect(component.items).toEqual(todos);
    });

    it('should add a new todo', () => {
        const todo = {id: 2, description: 'New Todo', isCompleted: false };
        spyOn(todoService, 'createTodoItem').and.returnValue(of(todo));
        component.newTodoItem = 'New Todo';

        component.handleAdd();

        expect(component.newTodoItem).toBe('');
    });

    it('should handle error when adding a new todo', () => {
        const errorMessage = 'Error adding todo'
        spyOn(todoService, 'createTodoItem').and.returnValue(throwError(() => errorMessage));
        component.newTodoItem = 'New Todo';

        component.handleAdd();

        expect(component.errorMessage).toBe(errorMessage);
    });
});