namespace MyFirstApi_cs.Router;

public static class ApiRoutes
{
    public static WebApplication MapApiRoutes(this WebApplication app)
    {
        app.MapControllers();
        return app;
    }
}