import { Mail, MapPin, User } from "lucide-react";
import { Button } from "../../app/layout/ui/Button";
import { useGetAddressesQuery } from "../../app/api/address/addressApi";
import { useAuth } from "../../hooks/useAuth";
import { useLazyGetFavoriteCountQuery } from "../../app/api/favorite/FavoriteApi";

export default function ProfilePage() {
  const { user } = useAuth();
  const { data: addresses } = useGetAddressesQuery();
  const {data: facoritesCount} = useLazyGetFavoriteCountQuery();

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-900 mb-8">My Profile</h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Profile Info */}
        <div className="lg:col-span-2 space-y-6">
          <div className="card p-6">
            <div className="flex items-center gap-6 mb-6">
              {user?.profileImageUrl ? (
                <img
                  src={user.profileImageUrl}
                  alt={user.firstName}
                  className="w-24 h-24 rounded-full object-cover"
                />
              ) : (
                <div className="w-24 h-24 rounded-full bg-primary-100 flex items-center justify-center">
                  <User size={40} className="text-primary-600" />
                </div>
              )}
              <div>
                <h2 className="text-2xl font-bold text-gray-900">
                  {user?.firstName} {user?.lastName}
                </h2>
                <p className="text-gray-600 flex items-center gap-2 mt-1">
                  <Mail size={16} />
                  {user?.email}
                </p>
              </div>
            </div>
            <Button>Edit Profile</Button>
          </div>

          {/* Addresses */}
          <div className="card p-6">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-xl font-semibold flex items-center gap-2">
                <MapPin size={20} />
                Saved Addresses
              </h3>
              <Button size="sm" className="bg-amber-500 hover:bg-amber-600">Add New</Button>
            </div>
            <div className="space-y-3">
              {addresses?.map((address) => (
                <div key={address.id} className="p-4 border border-gray-200 rounded-lg">
                  <div className="flex items-start justify-between">
                    <div>
                      <p className="font-medium">{address.label}</p>
                      <p className="text-sm text-gray-600 mt-1">
                        {address.street}<br />
                        {address.city},  {address.postalCode}
                      </p>
                    </div>
                    {address.isDefault && (
                      <span className="text-xs bg-primary-100 text-primary-700 px-2 py-1 rounded">
                        Default
                      </span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Quick Stats */}
        <div className="lg:col-span-1">
          <div className="card p-6">
            <h3 className="text-xl font-semibold mb-4">Quick Stats</h3>
            <div className="space-y-4">
              <div>
                <p className="text-sm text-gray-600">Total Orders</p>
                <p className="text-2xl font-bold">0</p>
              </div>
              <div>
                <p className="text-sm text-gray-600">Favorite Restaurants</p>
                <p className="text-2xl font-bold">{facoritesCount?.length || 0}</p>
              </div>
              <div>
                <p className="text-sm text-gray-600">Reviews Written</p>
                <p className="text-2xl font-bold">0</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}