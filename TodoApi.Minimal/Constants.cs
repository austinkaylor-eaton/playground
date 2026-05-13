namespace TodoApi.Minimal
{
    namespace Constants
    {
        /// <summary>
        /// Contains constants for the TodoItems endpoints, such as the tag used to group all endpoints under a single tag in the Swagger UI
        /// </summary>
        public static class TodoItems
        {
            /// <summary>
            /// Constant used to group all endpoints under a single tag in the Swagger UI
            /// </summary>
            public const string EndpointGroupTag = "TodoItems";

            /// <summary>
            /// Constant used to define the URL path for any TodoItem endpoints that retrieves a specific TodoItem by its ID
            /// </summary>
            /// <remarks>
            /// Used by the following endpoints: <br/>
            /// - GET /{id:int} <br/>
            /// - PUT /{id:int} <br/>
            /// - PATCH /{id:int} <br/>
            /// - DELETE /{id:int} <br/>
            /// </remarks>
            public const string IdUrlPath = "{id:int}";
        }
    }
}