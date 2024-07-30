import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";
import { environment } from "../environments/environment";

export interface TodoItem {
    id: number;
    description: string;
    isCompleted: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class TodoService {

    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) {}

    getTodos(): Observable<TodoItem[]> {
        return this.http.get<TodoItem[]>(`${this.apiUrl}`).pipe(
            catchError(this.handleError)
        );
    }

    getTodoById(id: number): Observable<TodoItem> {
        return this.http.get<TodoItem>(`${this.apiUrl}/${id}`).pipe(
            catchError(this.handleError)
        );
    }

    createTodoItem(todo: TodoItem): Observable<TodoItem> {
        return this.http.post<TodoItem>(`${this.apiUrl}`, todo).pipe(
            catchError(this.handleError)
        );
    }

    updateTodoItem(id: number, todo: TodoItem): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/${id}`, todo).pipe(
            catchError(this.handleError)
        );
    }

    markAsComplete(id: number): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/${id}/complete`, {}).pipe(
            catchError(this.handleError)
        );
    }

    deleteTodoItem(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
            catchError(this.handleError)
        );
    }

    private handleError(error: HttpErrorResponse) {
        let errorMessage = 'An unknown error occurred!';
        const message = error.error ? error.error.message : error.message;

        if (error.error instanceof ErrorEvent) {
            // Client-side or network error
            errorMessage = `Error: ${message}`;
        } else {
            // Backend error
            errorMessage = `Error code: ${error.status}\nMessage: ${message}`;
        }

        return throwError(() => errorMessage);
    }
}