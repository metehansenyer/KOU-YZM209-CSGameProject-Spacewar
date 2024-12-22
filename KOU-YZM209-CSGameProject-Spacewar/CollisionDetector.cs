using Raylib_cs;
using System.Numerics;

namespace KOU_YZM209_CSGameProject_Spacewar;

public static class CollisionDetector
{
    // Checks for collision between player spaceship and enemy spaceship
    public static bool CheckCollision(Spaceship player, Enemy enemy)
    {
        // Get the rotated vertices of the player's and enemy's spaceship
        Vector2[] playerVertices = GetRotatedSpaceshipVertices(player.playersShipsDes, player.rotation);
        Vector2[] enemyVertices = GetRotatedSpaceshipVertices(enemy.enemyShipsDes, enemy.rotationAngle);
        
        // Check for triangle collision between player and enemy
        return CheckTriangleCollision(playerVertices[0], playerVertices[1], playerVertices[2], enemyVertices[0], enemyVertices[1], enemyVertices[2]);
    }
    
    // Checks if any bullet from the player hits an enemy
    public static void CheckBulletCollision(List<Bullet> bullets, List<Enemy> enemies, SoundManager soundManager)
    {
        // Iterate through each bullet and enemy to check for collisions
        foreach (var bullet in bullets)
        {
            foreach (var enemy in enemies)
            {
                // Get the rotated vertices of the enemy's spaceship
                Vector2[] enemyVertices = GetRotatedSpaceshipVertices(enemy.enemyShipsDes, enemy.rotationAngle);
                
                // Define the bullet's center and radius
                Vector2 bulletCenter = new Vector2(bullet.bulletDes.X, bullet.bulletDes.Y);
                float bulletRadius = bullet.bulletDes.Width / 10;
                
                // Check if the bullet collides with the enemy's spaceship
                if (CheckCollisionCircleTriangle(bulletCenter, bulletRadius, enemyVertices[0], enemyVertices[1], enemyVertices[2]))
                {
                    // Play collision sound and handle damage
                    soundManager.PlayBulletCollisionSound();
                    
                    enemy.TakeDamage(bullet.damage);
                    bullet.OnHit();
                    break;
                }
            }
        }
    }
    
    // Checks if any bullet from the enemy hits the player
    public static void CheckEnemyBulletCollision(Spaceship player, List<Bullet> bullets, SoundManager soundManager)
    {
        // Iterate through each enemy bullet to check for collisions
        foreach (var bullet in bullets)
        {
            // Get the rotated vertices of the player's spaceship
            Vector2[] playerVertices = GetRotatedSpaceshipVertices(player.playersShipsDes, player.rotation);
            
            // Define the bullet's center and radius
            Vector2 bulletCenter = new Vector2(bullet.bulletDes.X, bullet.bulletDes.Y);
            float bulletRadius = bullet.bulletDes.Width / 10;

            // Check if the bullet is of type EnergyBullet and if it has exploded
            if (bullet is EnergyBullet energyBullet)
            {
                if (energyBullet.hasExploded)
                {
                    // Define the explosion area and check for collision with player
                    Rectangle dest = new Rectangle(energyBullet.playerPosition.X - 64, energyBullet.playerPosition.Y - 64, 128, 128);
                    
                    // Check for collision between explosion and player's spaceship
                    if (CheckCollisionRecTriangle(dest, playerVertices[0], playerVertices[1], playerVertices[2]))
                    {
                        if (!player.isInvisibilityActive)
                        {
                            // Play collision sound and handle damage
                            soundManager.PlayBulletCollisionSound();
                    
                            player.TakeDamage(energyBullet.damage * 3 / 2); // Player takes increased damage
                            energyBullet.damage = 0;
                        }
                        
                        break;
                    }
                }
            }
            
            // Check if a regular bullet collides with the player's spaceship
            if (CheckCollisionCircleTriangle(bulletCenter, bulletRadius, playerVertices[0], playerVertices[1], playerVertices[2]))
            {
                if (!player.isInvisibilityActive)
                {
                    // Play collision sound and handle damage
                    soundManager.PlayBulletCollisionSound();
                    
                    player.TakeDamage(bullet.damage);
                    bullet.OnHit(); // Bullet destroyed
                }
                
                break;
            }
        }
    }

    // Checks if the player collects any collectible items
    public static void CheckPlayerCollectibleCollision(Spaceship player, List<Collectible> collectibles, SoundManager soundManager)
    {
        // Iterate through each collectible to check for collisions with player
        foreach (var collectible in collectibles)
        {
            // Get the rotated vertices of the player's spaceship
            Vector2[] playerVertices = GetRotatedSpaceshipVertices(player.playersShipsDes, player.rotation);
            
            // Define the collectible's center and radius
            Vector2 collectibleCenter = new Vector2(collectible.collectibleDes.X, collectible.collectibleDes.Y);
            float collectibleRadius = collectible.collectibleDes.Width / 10;
            
            // Check if the collectible collides with the player's spaceship
            if (CheckCollisionCircleTriangle(collectibleCenter, collectibleRadius, playerVertices[0], playerVertices[1], playerVertices[2]))
            {
                // Play collectible sound and handle collection
                soundManager.PlayCoinSound();
                
                collectible.OnCollect(player);
                break;
            }
        }
    }
    
    // Helper method to calculate rotated spaceship vertices
    private static Vector2[] GetRotatedSpaceshipVertices(Rectangle spaceshipDes, float rotation)
    {
        Vector2
            center = new Vector2(spaceshipDes.X, spaceshipDes.Y),
            triA = new Vector2(spaceshipDes.X, spaceshipDes.Y - spaceshipDes.Height / 2),
            triB = new Vector2(spaceshipDes.X - spaceshipDes.Width / 2, spaceshipDes.Y + spaceshipDes.Height / 2),
            triC = new Vector2(spaceshipDes.X + spaceshipDes.Width / 2, spaceshipDes.Y + spaceshipDes.Height / 2),
            rotatedA = Utils.RotatePoint(triA, center, rotation),
            rotatedB = Utils.RotatePoint(triB, center, rotation),
            rotatedC = Utils.RotatePoint(triC, center, rotation);
        
        Vector2[] vertices = new Vector2[3] { rotatedA, rotatedB, rotatedC };

        // Rotate the vertices based on the spaceship's rotation
        return vertices;
    }
    
    // Checks for collision between a circle and a triangle
    private static bool CheckCollisionCircleTriangle(Vector2 circleCenter, float radius, Vector2 triA, Vector2 triB, Vector2 triC)
    {
        // Check for collision between circle and each side of the triangle
        if (IsCircleCollidingWithLine(circleCenter, radius, triA, triB) ||
            IsCircleCollidingWithLine(circleCenter, radius, triB, triC) ||
            IsCircleCollidingWithLine(circleCenter, radius, triC, triA))
        {
            return true;
        }
        
        // Check if the circle's center is inside the triangle
        if (IsPointInsideTriangle(circleCenter, triA, triB, triC))
        {
            return true;
        }

        return false;
    }
    
    // Checks for collision between a rectangle and a triangle
    private static bool CheckCollisionRecTriangle(Rectangle rect, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        // Define the four corners of the rectangle
        Vector2 rectTopLeft = new Vector2(rect.X, rect.Y);
        Vector2 rectTopRight = new Vector2(rect.X + rect.Width, rect.Y);
        Vector2 rectBottomLeft = new Vector2(rect.X, rect.Y + rect.Height);
        Vector2 rectBottomRight = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
        
        // Check for collision between the rectangle's corners and the triangle
        if (Raylib.CheckCollisionPointTriangle(rectTopLeft, v1, v2, v3) ||
            Raylib.CheckCollisionPointTriangle(rectTopRight, v1, v2, v3) ||
            Raylib.CheckCollisionPointTriangle(rectBottomLeft, v1, v2, v3) ||
            Raylib.CheckCollisionPointTriangle(rectBottomRight, v1, v2, v3))
        {
            return true;
        }

        // Check if any sides of the rectangle intersect with the triangle
        if (DoLinesIntersect(rectTopLeft, rectTopRight, v1, v2) ||
            DoLinesIntersect(rectTopLeft, rectTopRight, v2, v3) ||
            DoLinesIntersect(rectTopLeft, rectTopRight, v3, v1) ||
            DoLinesIntersect(rectTopRight, rectBottomRight, v1, v2) ||
            DoLinesIntersect(rectTopRight, rectBottomRight, v2, v3) ||
            DoLinesIntersect(rectTopRight, rectBottomRight, v3, v1) ||
            DoLinesIntersect(rectBottomRight, rectBottomLeft, v1, v2) ||
            DoLinesIntersect(rectBottomRight, rectBottomLeft, v2, v3) ||
            DoLinesIntersect(rectBottomRight, rectBottomLeft, v3, v1) ||
            DoLinesIntersect(rectBottomLeft, rectTopLeft, v1, v2) ||
            DoLinesIntersect(rectBottomLeft, rectTopLeft, v2, v3) ||
            DoLinesIntersect(rectBottomLeft, rectTopLeft, v3, v1))
        {
            return true;
        }
        
        // Check if all points of the triangle are inside the rectangle
        if (Raylib.CheckCollisionPointRec(v1, rect) &&
            Raylib.CheckCollisionPointRec(v2, rect) &&
            Raylib.CheckCollisionPointRec(v3, rect))
        {
            return true;
        }

        return false;
    }
    
    // Checks if a circle is colliding with a line segment
    private static bool IsCircleCollidingWithLine(Vector2 circleCenter, float radius, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 lineDir = lineEnd - lineStart; // Direction of the line segment
        float lineLength = lineDir.Length(); // Length of the line segment
        lineDir /= lineLength; // Normalize the direction

        Vector2 centerToStart = circleCenter - lineStart;  // Vector from line start to circle center
        float projection = Vector2.Dot(centerToStart, lineDir); // Project the center to the line
        projection = Math.Clamp(projection, 0, lineLength); // Clamp the projection to the segment

        Vector2 closestPoint = lineStart + lineDir * projection; // Find the closest point on the line
        
        float distanceToCircle = Vector2.Distance(circleCenter, closestPoint); // Distance from the circle to the closest point
        return distanceToCircle <= radius; // Check if the distance is less than the radius
    }
    
    // Checks if a point is inside a triangle
    private static bool IsPointInsideTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float denominator = (b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y); // Denominator for barycentric coordinates
        float alpha = ((b.Y - c.Y) * (p.X - c.X) + (c.X - b.X) * (p.Y - c.Y)) / denominator;
        float beta = ((c.Y - a.Y) * (p.X - c.X) + (a.X - c.X) * (p.Y - c.Y)) / denominator;
        float gamma = 1.0f - alpha - beta; // Ensure the sum of alpha, beta, and gamma is 1
        
        return alpha >= 0 && beta >= 0 && gamma >= 0; // Check if the point is inside the triangle
    }
    
    // Checks for collision between two triangles
    private static bool CheckTriangleCollision(Vector2 a1, Vector2 b1, Vector2 c1, Vector2 a2, Vector2 b2, Vector2 c2)
    {
        // Check if any edge of the two triangles intersects
        if (DoLinesIntersect(a1, b1, a2, b2) || DoLinesIntersect(a1, b1, b2, c2) || DoLinesIntersect(a1, b1, c2, a2) ||
            DoLinesIntersect(b1, c1, a2, b2) || DoLinesIntersect(b1, c1, b2, c2) || DoLinesIntersect(b1, c1, c2, a2) ||
            DoLinesIntersect(c1, a1, a2, b2) || DoLinesIntersect(c1, a1, b2, c2) || DoLinesIntersect(c1, a1, c2, a2))
        {
            return true;
        }
        
        // Check if any vertex of the first triangle is inside the second triangle
        if (IsPointInsideTriangle(a1, a2, b2, c2) || IsPointInsideTriangle(b1, a2, b2, c2) || IsPointInsideTriangle(c1, a2, b2, c2))
        {
            return true;
        }
        
        // Check if any vertex of the second triangle is inside the first triangle
        if (IsPointInsideTriangle(a2, a1, b1, c1) || IsPointInsideTriangle(b2, a1, b1, c1) || IsPointInsideTriangle(c2, a1, b1, c1))
        {
            return true;
        }

        return false; // Return false if no collision is detected
    }
    
    // Checks if two line segments intersect
    private static bool DoLinesIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        // Helper function to calculate the orientation of the triplet (a, b, c)
        float Orientation(Vector2 a, Vector2 b, Vector2 c)
        {
            return (b.Y - a.Y) * (c.X - b.X) - (b.X - a.X) * (c.Y - b.Y); // Cross product to determine the orientation
        }

        // Calculate the orientation of the triplets formed by the line segments
        float o1 = Orientation(p1, q1, p2);
        float o2 = Orientation(p1, q1, q2);
        float o3 = Orientation(p2, q2, p1);
        float o4 = Orientation(p2, q2, q1);
        
        // If the orientations are different, the lines intersect
        return (o1 * o2 < 0 && o3 * o4 < 0);
    }
}