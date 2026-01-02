import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useNavigate } from "react-router-dom";
import { useCreateRestaurantMutation } from "../../app/api/restaurant/restaurantApi";
import { AddressAutocomplete } from "../address/AddressAutoComplete";
import { CreateRestaurantSchema, type CreateRestaurantFormValues } from "../../utils/validators/createRestaurantSchema";
import { Input } from "../../app/layout/ui/Input";
import { Button } from "../../app/layout/ui/Button";
import { toast } from "react-toastify";

type AddressAutocompleteResult = {
  street: string;
  city: string;
  state?: string;
  postalCode?: string;
  countryCode: string;
  latitude: number;
  longitude: number;
};


export default function CreateRestaurantForm() {
  const navigate = useNavigate();
  const [createRestaurant, { isLoading }] = useCreateRestaurantMutation();

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
    clearErrors,
  } = useForm<CreateRestaurantFormValues>({
    resolver: zodResolver(CreateRestaurantSchema),
    defaultValues: {
      deliveryFee: 0,
      estimatedDeliveryTime: 30,
      deliveryRadiusKm: 5,
    },
  });

  const normalizePhone = (phone: string) =>
  phone.replace(/[^\d+]/g, "");


  const onSubmit = async (data: CreateRestaurantFormValues) => {
    try {
      normalizePhone(data.phone);
      const result = await createRestaurant(data).unwrap();
      toast.success("Restaurant Added!")
      
      navigate(`/restaurants/${result.data.id}`);
      console.log(result.data.id);
      
      
    } catch (error) {
      toast.error("Failed Adding Restaurant!")
      
    }
    
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="space-y-5 max-w-xl mx-auto"
    >
      <h1 className="font-serif font-bold text-2xl mt-3.5">Register Your Restaurant</h1>
      {/* NAME */}
      <div>
        <Input
          {...register("name")}
          placeholder="Restaurant name"
          className="input"
        />
        {errors.name && <p className="error">{errors.name.message}</p>}
      </div>

      {/* DESCRIPTION */}
      <div>
        <textarea
          {...register("description")}
          placeholder="Describe your restaurant"
          className="textarea"
        />
        {errors.description && (
          <p className="error">{errors.description.message}</p>
        )}
      </div>

      {/* PHONE */}
      <div>
        <Input
          {...register("phone")}
          placeholder="Phone number"
          className="input"
        />
        {errors.phone && <p className="error">{errors.phone.message}</p>}
      </div>

      {/* DELIVERY FEE */}
      <div>
        <Input
          type="number"
          min={0}
          step="0.1"
          {...register("deliveryFee", { valueAsNumber: true })}
          placeholder="Delivery fee"
          className="input"
        />
        {errors.deliveryFee && (
          <p className="error">{errors.deliveryFee.message}</p>
        )}
      </div>

      {/* DELIVERY TIME */}
      <div>
        <Input
          type="number"
          min={1}
          {...register("estimatedDeliveryTime", { valueAsNumber: true })}
          placeholder="Estimated delivery time (minutes)"
          className="input"
        />
        {errors.estimatedDeliveryTime && (
          <p className="error">{errors.estimatedDeliveryTime.message}</p>
        )}
      </div>

      {/* DELIVERY RADIUS */}
      <div>
        <Input
          type="number"
          min={1}
          {...register("deliveryRadiusKm", { valueAsNumber: true })}
          placeholder="Delivery radius (km)"
          className="input"
        />
        {errors.deliveryRadiusKm && (
          <p className="error">{errors.deliveryRadiusKm.message}</p>
        )}
      </div>

      {/* ADDRESS AUTOCOMPLETE */}
      <div>
        <label className="label">Address</label>
        <AddressAutocomplete
          onSelect={(address) => {
            setValue(
              "addressDto",
              {
                label: "Main",
                street: address.street,
                city: address.city,
                state: address.state,
                postalCode: address.postalCode,
                countryCode: address.countryCode,
                latitude: address.latitude,
                longitude: address.longitude,
              },
              { shouldValidate: true }
            );
            clearErrors("addressDto");
          }}
        />
        {errors.addressDto && (
          <p className="error">Valid address is required</p>
        )}
      </div>

      {/* SUBMIT */}
      <Button
        disabled={isLoading}
       variant="amber"
       className="font-serif mx-auto"
      >
        {isLoading ? "Creating..." : "Create Restaurant"}
      </Button>
    </form>
  );
}
