# NotesApplication

Feature Overview:
1. Create a Note
    * Users should be able to create a note with the following fields:
    * Title (mandatory)
    * Content (optional)
    * Created At (auto-generated date)
    * Updated At (auto-generated when edited)
2. Read Notes
    *  Users can view a list of their notes, showing the title and creation date.
    * Users can click on a note to view its content and full details.
3. Update a Note
    * Users can edit the title and content of a note.
    *   The Updated At timestamp should be updated whenever the note is modified.
4. Delete a Note
    * Users can delete any note.
    * After deletion, the note should be removed from the list.

Technical Stack:
- Front-End: Vue, typescript + tailwind
- Back-End: C# (asp.net core web API)
- Database: Sql Server

Functional Requirements:
1. Frontend
    * Notes list page which cover all of this Create, read, update, and delete (CRUD) operation.
    * Simple filtering and sorting functionality
    * Search
    * Responsive design using TailwindCss
    * Perform basic API integrations using Axios
    * State management
2. Backend
    * Create, read, update, and delete (CRUD) notes
    * user can only read, update and delete their own notes
    * using Dapper for ORM with a SQL Server database